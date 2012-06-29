﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MayhemCore;
using MayhemWebCamWrapper;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MayhemVisionModules.Components
{
    //buffers images from a webcam... used, for instance, in WebCamSnapshot reaction
    public class WebCamBuffer : ImageListener, IBufferingImager
    {
        // update the loop only every quarter second -- this should be sufficient for the Picture Event       
        public const double LoopBufferUpdateMs = 250.0;

        // store LOOP_DURATION ms of footage in the past/future
        public const int LoopDuration = 30000;

        private DateTime loopBufferLastUpdate;
        private readonly int loopBufferMaxLength;

        //fifo buffer that stores last x images
        private readonly Queue<BitmapTimeStamp> loopBuffer;

        public Bitmap ImageAsBitmap(byte[] data)
        {
            int w = Convert.ToInt16(ImagerWidth);
            int h = Convert.ToInt16(ImagerHeight);
            try
            {
                Bitmap backBuffer = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Rectangle rect = new Rectangle(0, 0, w, h);

                System.Drawing.Imaging.BitmapData bmpData =
                    backBuffer.LockBits(
                    rect,
                    System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    backBuffer.PixelFormat);
                IntPtr imgPtr = bmpData.Scan0;

                System.Runtime.InteropServices.Marshal.Copy(data, 0, imgPtr, (int)(ImagerWidth * ImagerHeight * 3));
                backBuffer.UnlockBits(bmpData);

                return backBuffer;
            }
            catch (ArgumentException ex)
            {
                Logger.WriteLine("ArgumentException: " + ex);
                return new Bitmap(w, h);
            }
        }

        public WebCamBuffer()
        {
            loopBufferLastUpdate = DateTime.Now;
            loopBufferMaxLength = LoopDuration / 30; //TO DO - substitute 30 with camera frame rate
            loopBuffer = new Queue<BitmapTimeStamp>();
        }

        public override void UpdateFrame(object sender, EventArgs e)
        {
            try
            {
                WebCam camera = sender as WebCam;
                DateTime now = DateTime.Now;
                TimeSpan lastUpdate = now - loopBufferLastUpdate;
                if (lastUpdate.TotalMilliseconds >= LoopBufferUpdateMs)
                {
                    loopBufferLastUpdate = DateTime.Now;
                    if (loopBuffer.Count < loopBufferMaxLength)
                    {
                        if (ImagerWidth != default(double) && ImagerHeight != default(double) && camera.ImageBuffer != null)
                        {
                            loopBuffer.Enqueue(new BitmapTimeStamp(ImageAsBitmap(camera.ImageBuffer)));
                        }
                    }
                    else
                    {
                        BitmapTimeStamp destroyMe = loopBuffer.Dequeue();
                        destroyMe.Dispose();
                        if (ImagerWidth != default(double) && ImagerHeight != default(double) && camera.ImageBuffer != null)
                        {
                            loopBuffer.Enqueue(new BitmapTimeStamp(ImageAsBitmap(camera.ImageBuffer)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void ClearBuffer()
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, (SendOrPostCallback)delegate
            {
                loopBuffer.Clear();
            }, null);
        }

        public System.Drawing.Bitmap GetLastBufferedItem()
        {
            try
            {
                int tailIdx = loopBuffer.Count - 1;
                if (tailIdx > 0 && tailIdx < loopBuffer.Count)
                {
                    return loopBuffer.ElementAt(tailIdx).Image;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public System.Drawing.Bitmap GetBufferItemAtIndex(int index)
        {
            int tailIdx = loopBuffer.Count - 1 - index;
            if (tailIdx > 0 && tailIdx < loopBuffer.Count)
            {
                return loopBuffer.ElementAt(tailIdx).Image;
            }
            return null;
        }

        public DateTime GetBufferTimeStampAtIndex(int index)
        {
            int tailIdx = loopBuffer.Count - 1 - index;
            if (tailIdx < loopBuffer.Count)
            {
                return loopBuffer.ElementAt(tailIdx).TimeStamp;
            }

            return DateTime.MinValue;
        }
    }
}
