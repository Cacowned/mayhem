using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using AviFile;
using MayhemCore;
using MayhemOpenCVWrapper.LowLevel;

namespace MayhemOpenCVWrapper
{
    /// <summary>
    /// Allows Mayhem to write AVI files from lists of Bitmaps
    /// 
    /// Uses AviFile Library by Corinna John
    /// http://www.codeproject.com/KB/audio-video/avifilewrapper.aspx
    /// Licensed as FOSS under Code Project Open License: http://www.codeproject.com/info/cpol10.aspx
    /// </summary>
    public class Video
    {
        public delegate void VideoSavedEventHandler(object sender, VideoSavedEventArgs e);

        public event VideoSavedEventHandler OnVideoSaved;

        private readonly List<Bitmap> videoFrames;

        // video stream settings
        private readonly double frameRate;

        // TODO: These are never being used
        private int width;
        private int height;

        // video stream
        private readonly VideoStream stream;
        private readonly AviManager aviManager;
        private int frames;

        /// <summary>
        /// Initializes a new video object.
        /// The video object opens a file path and builds a video file with the contents of the image frames from the camera's disk buffer. 
        /// </summary>
        /// <param name="c">camera object</param>
        /// <param name="fileName">path to video file</param>
        /// <param name="compress">compress the video file or not (affects video encoding speed)</param>
        public Video(Camera c, string fileName, bool compress)
        {
            Camera camera = c;
            frameRate = 1000 / camera.Settings.UpdateRateMs;
            width = c.Settings.ResX;
            height = c.Settings.ResY;

            // preserve reference to the camera frames to be saved later
            videoFrames = c.VideoDiskBufferItems;

            if (videoFrames.Count > 0)
            {
                aviManager = new AviManager(fileName, false);

                Avi.AVICOMPRESSOPTIONS opts = new Avi.AVICOMPRESSOPTIONS();
                opts.fccType = (UInt32)Avi.mmioStringToFOURCC("vids", 0);
                opts.fccHandler = (UInt32)Avi.mmioStringToFOURCC("CVID", 0);
                opts.dwKeyFrameEvery = 0;
                opts.dwQuality = 1000;  // 0 .. 10000
                opts.dwFlags = 0;  // AVICOMRPESSF_KEYFRAMES = 4
                opts.dwBytesPerSecond = 0;
                opts.lpFormat = new IntPtr(0);
                opts.cbFormat = 0;
                opts.lpParms = new IntPtr(0);
                opts.cbParms = 0;
                opts.dwInterleaveEvery = 0;

                // false creates a new file
                if (compress)
                {
                    Logger.WriteLine("Saving Compressed");
                    stream = aviManager.AddVideoStream(opts, frameRate, videoFrames[0]);     // add first frame as an example of the video's format
                }
                else
                {
                    Logger.WriteLine("Saving Uncompressed");
                    stream = aviManager.AddVideoStream(false, frameRate, videoFrames[0]);
                }

                // add the frames
                ThreadPool.QueueUserWorkItem(AddFrames);
            }
        }

        /// <summary>
        /// Write out the video in a background thread
        /// </summary>
        /// <param name="state"></param>
        private void AddFrames(object state)
        {
            Logger.WriteLine("Adding Frames");
            foreach (Bitmap img in videoFrames)
            {
                stream.AddFrame(img);
                frames++;
            }

            aviManager.Close();

            foreach (Bitmap img in videoFrames)
            {
                img.Dispose();
            }

            Logger.WriteLine("Created AVI with " + frames + " frames.");
            if (OnVideoSaved != null)
            {
                OnVideoSaved(this, new VideoSavedEventArgs(true));
            }
        }
    }
}
