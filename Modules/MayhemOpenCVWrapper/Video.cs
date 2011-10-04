﻿/*
 * Video.cs
 * Allows Mayhem to write AVI files from lists of Bitmaps
 * 
 * Uses AviFile Library by Corinna John
 * http://www.codeproject.com/KB/audio-video/avifilewrapper.aspx
 * Licensed as FOSS under Code Project Open License: http://www.codeproject.com/info/cpol10.aspx
 * 
 * (c) 2011, Microsoft Applied Scienced Group
 * 
 * Author: Sven Kratz
 * 
 */
using System;
using System.Collections.Generic;
using System.Threading;
using AviFile;
using MayhemCore;

namespace MayhemOpenCVWrapper
{
    public class Video
    {
        private List<BitmapTimestamp> video_frames = new List<BitmapTimestamp>();

        // video stream settings
        private double frameRate;
        private int width;
        private int height;

        // video stream
        VideoStream stream = null;
        AviManager aviManager = null;
        private int frames = 0;

        public event Action<bool> OnVideoSaved;
        
        public Video(Camera c, string fileName, bool compress)
        {
            Camera camera = c;
            frameRate = 1000 / camera.Settings.UpdateRateMs;
            width = c.Settings.ResX;
            height = c.Settings.ResY; 

            // preserve reference to the camera frames to be saved later

            video_frames = c.BufferItems;

            if (video_frames.Count > 0)
            {
                aviManager = new AviManager(fileName,false);

                Avi.AVICOMPRESSOPTIONS opts = new Avi.AVICOMPRESSOPTIONS();
                opts.fccType         = (UInt32)Avi.mmioStringToFOURCC("vids", 0);
                opts.fccHandler      = (UInt32)Avi.mmioStringToFOURCC("CVID", 0);
                opts.dwKeyFrameEvery = 0;
                opts.dwQuality       = 1000;  // 0 .. 10000
                opts.dwFlags         = 0;  // AVICOMRPESSF_KEYFRAMES = 4
                opts.dwBytesPerSecond= 0;
                opts.lpFormat        = new IntPtr(0);
                opts.cbFormat        = 0;
                opts.lpParms         = new IntPtr(0);
                opts.cbParms         = 0;
                opts.dwInterleaveEvery = 0;

                // false creates a new file
                if (compress)
                {
                    Logger.WriteLine("Saving Compressed");
                    stream = aviManager.AddVideoStream(opts, frameRate, video_frames[0].Image);     // add first frame as an example of the video's format
                }
                else
                {
                    Logger.WriteLine("Saving Uncompressed");
                    stream = aviManager.AddVideoStream(false, frameRate, video_frames[0].Image); 
                }
              
                // add the frames
                ThreadPool.QueueUserWorkItem(TAddFrames);         
            }
        }

        /// <summary>
        /// Write out the video in a background thread
        /// </summary>
        /// <param name="state"></param>
        private void TAddFrames(object state)
        {
            Logger.WriteLine("Adding Frames");
            foreach (BitmapTimestamp img in video_frames)
            {
                stream.AddFrame(img.Image);
                frames++;
            }
            aviManager.Close();
            

            foreach (BitmapTimestamp img in video_frames)
            {
                img.Dispose(); 
            }

            Logger.WriteLine("Created AVI with " + frames + " frames.");
            if (this.OnVideoSaved != null)
            {
                OnVideoSaved(true);
            }
        }     
    }
}
