﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using KinectModules.Wpf;
using System.Runtime.Serialization;
using Microsoft.Research.Kinect.Nui;
using System.Windows.Media;
using DTWGestureRecognition;
using System.Collections;
using KinectModules.Helpers;
using System.IO;

namespace KinectModules
{
    [DataContract]
    [MayhemModule("Kinect Event", "Triggers when a Kinect gesture is detected")]
    public class KinectSkeletalGesture : EventBase, IWpfConfigurable
    {
        private const string gestureFileLocation = "Packages\\KinectModules\\GesturesDtw\\GesturesDTW.txt";

        /// <summary>
        /// The minumum number of frames in the _video buffer before we attempt to start matching gestures
        /// </summary>
        private const int MinimumFrames = 6;

        /// <summary>
        /// How many skeleton frames to store in the _video buffer
        /// </summary>
        private const int BufferSize = 32;

        /// <summary>
        /// How many skeleton frames to ignore (_flipFlop)
        /// 1 = capture every frame, 2 = capture every second frame etc.
        /// </summary>
        private const int Ignore = 2;

        /// <summary>
        /// Switch used to ignore certain skeleton frames
        /// </summary>
        private int _flipFlop;

        private ArrayList _video;
        private DtwGestureRecognizer _dtw;

        private MayhemKinect kinect;
        private EventHandler<SkeletonFrameReadyEventArgs> skeletonFrameHandler;

  

        protected override void OnAfterLoad()
        {
            _video = new ArrayList();
            _dtw = new DtwGestureRecognizer(12, 0.6, 2, 2, 10);
            _flipFlop = 0;

            kinect = MayhemKinect.Instance;
            skeletonFrameHandler = new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonExtractSkeletonFrameReady);

            // skeleton data extractor
            Skeleton2DDataExtract.Skeleton2DdataCoordReady += NuiSkeleton2DdataCoordReady;

            string gestureFilePath = Directory.GetCurrentDirectory() + "\\" + gestureFileLocation;

            // read gesture file
            if (_dtw.LoadGesturesFromFile(gestureFilePath))
            { }
            else
            {
                ErrorLog.AddError(ErrorType.Failure, "Could not read gesture definition file!");
            }
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            kinect.AttachSkeletonEventHandler(skeletonFrameHandler);
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            kinect.DetachSkeletonEventHandler(skeletonFrameHandler);
        }

        public MayhemWpf.UserControls.WpfConfiguration ConfigurationControl
        {
            get 
            {
                return new KinectEventConfig();
            }
        }

        public void OnSaved(MayhemWpf.UserControls.WpfConfiguration configurationControl)
        {
            throw new NotImplementedException();
        }

        public string GetConfigString()
        {
            throw new NotImplementedException();
        }

        
        /// <summary>
        /// Called each time a skeleton frame is ready. Passes skeletal data to the DTW processor
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Skeleton Frame Ready Event Args</param>
        private static void SkeletonExtractSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame skeletonFrame = e.SkeletonFrame;
            foreach (SkeletonData data in skeletonFrame.Skeletons)
            {
                Skeleton2DDataExtract.ProcessData(data);
            }
        }

        /// <summary>
        /// Runs every time our 2D coordinates are ready.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="a">Skeleton 2Ddata Coord Event Args</param>
        private void NuiSkeleton2DdataCoordReady(object sender, Skeleton2DdataCoordEventArgs a)
        {
           

            // We need a sensible number of frames before we start attempting to match gestures against remembered sequences
            if (_video.Count > MinimumFrames )
            {
                ////Debug.WriteLine("Reading and video.Count=" + video.Count);
                string s = _dtw.Recognize(_video);
                //results.Text = "Recognised as: " + s;
                if (!s.Contains("__UNKNOWN"))
                {
                    // There was no match so reset the buffer
                    _video = new ArrayList();
                }
            }

            // Ensures that we remember only the last x frames
            if (_video.Count > BufferSize)
            {
                
                // Remove the first frame in the buffer
                _video.RemoveAt(0);
                
            }

            // Decide which skeleton frames to capture. Only do so if the frames actually returned a number. 
            // For some reason my Kinect/PC setup didn't always return a double in range (i.e. infinity) even when standing completely within the frame.
            // TODO Weird. Need to investigate this
            if (!double.IsNaN(a.GetPoint(0).X))
            {
                // Optionally register only 1 frame out of every n
                _flipFlop = (_flipFlop + 1) % Ignore;
                if (_flipFlop == 0)
                {
                _video.Add(a.GetCoords());
                }
            }

            // Update the debug window with Sequences information
            //dtwTextOutput.Text = _dtw.RetrieveText();
        }

        

    }
}
