using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemOpenCVWrapper.LowLevel;
using System.Windows;
using MayhemOpenCVWrapper;
using System.Diagnostics;

namespace VisionModules.Events.Components
{
    public class MotionDetectorComponent : CameraImageListener
    {
        public event EventHandler OnMotionUpdate;

        public Rect motionBoundaryRect = new Rect();

        private int width;
        private int height;

        public MotionDetectorComponent(ImagerBase camera)
        {
            this.camera = camera;
            this.width = camera.Settings.ResX;
            this.height = camera.Settings.ResY;
        }

        double? oldAverage = null;
        double numberOfMotionFrames = 0;
        double threshold = 0.5;

        Queue<double> runningAverage = new Queue<double>(10);

        int hitCount;

        DateTime lastMovement = DateTime.Now;
        TimeSpan settleTime = TimeSpan.FromSeconds(1);
        private ImagerBase camera;

        public override void UpdateFrame(object sender, EventArgs e)
        {
            Camera camera = sender as Camera;

            int stride = camera.ImageBuffer.Length / height;

            double average = 0;

            lock (camera.ThreadLocker)
            {
                for (int x = (int)motionBoundaryRect.Left; x < (int)motionBoundaryRect.Right; x++)
                {
                    for (int y = (int)motionBoundaryRect.Top; y < (int)motionBoundaryRect.Bottom; y++)
                    {
                        average += camera.ImageBuffer[x + y * stride];
                    }
                }
            }

            average /= camera.ImageBuffer.Length;

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
