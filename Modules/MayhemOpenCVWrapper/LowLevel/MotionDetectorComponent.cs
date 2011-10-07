/*
 * PresenceDetectorComponent.cs
 * 
 * Low level object interfacing with the OpenCVDLL motion detector. 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;

using Point = System.Drawing.Point;
using MayhemOpenCVWrapper;
using MayhemCore;
using System.Threading;


namespace MayhemOpenCVWrapper.LowLevel
{
    /* <summary>
     * Wrapper for the C++ Motion Detector
     *  TODO: Make the detector classes more generic
     * <summary>
     * */
    public class MotionDetectorComponent : IVisionEventComponent
    {
        public event EventHandler OnMotionUpdate;

        public Rect motionBoundaryRect = new Rect();

        private int width;
        private int height;

        public MotionDetectorComponent(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        double? oldAverage = null;
        double numberOfMotionFrames = 0;
        double threshold = 0.5;

        Queue<double> runningAverage = new Queue<double>(10);

        int hitCount;

        DateTime lastMovement = DateTime.Now;
        TimeSpan settleTime = TimeSpan.FromSeconds(0.5);

        public override void update_frame(object sender, EventArgs e)
        {
            Camera camera = sender as Camera;

            int stride = camera.imageBuffer.Length / height;

            double average = 0;

            lock (camera.thread_locker)
            {
                for (int x = (int)motionBoundaryRect.Left; x < (int)motionBoundaryRect.Right; x++)
                {
                    for (int y = (int)motionBoundaryRect.Top; y < (int)motionBoundaryRect.Bottom; y++)
                    {
                        average += camera.imageBuffer[x + y * stride];
                    }
                }
            }

            average /= camera.imageBuffer.Length;

            threshold = 0;

            foreach (var frame in runningAverage)
            {
                threshold += frame;
            }

            threshold /= runningAverage.Count;

            if (oldAverage.HasValue)
            {
                double difference = Math.Abs(oldAverage.Value - average);

                if (runningAverage.Count > 10)
                {
                    runningAverage.Dequeue();
                }

                runningAverage.Enqueue(difference);

                if (difference > threshold * Sensitivity)
                {
                    if (DateTime.Now > lastMovement.Add(settleTime))
                    {
                        if (numberOfMotionFrames == 0)
                        {
                            Debug.WriteLine("Hit Count: {0}", hitCount++);
                            if (OnMotionUpdate != null)
                                OnMotionUpdate(this, EventArgs.Empty);
                        }
                    }
                    lastMovement = DateTime.Now;
                }
            }

            oldAverage = average;
        }

        public void SetMotionBoundaryRect(Rect r)
        {
            if (!r.IsEmpty || r.Width != 0 || r.Height != 0)
            {
                motionBoundaryRect = r;
            }
            else
            {
                // default to an "empty" rectangle
                motionBoundaryRect = new Rect(0, 0, 0, 0);
            }
        }

        public double Sensitivity { get; set; }
    }
}
