using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AviFile;
using System.Threading;
using MayhemCore;

namespace MayhemOpenCVWrapper
{
    class Video
    {
        double offset = 0; 
        private Camera camera = null;
        private List<BitmapTimestamp> video_frames = new List<BitmapTimestamp>();

        // video stream settings
        private double frameRate;
        private int width;
        private int height;
        private bool isCompressed = true; 

        // video stream
        VideoStream stream = null;
        AviManager aviManager = null;

        private int frames = 0;


        public event Action<bool> OnVideoSaved;
        

        public Video(Camera c, string fileName)
        {
            Camera camera = c;
            frameRate = 1000 / camera.settings.updateRate_ms;
            width = c.settings.resX;
            height = c.settings.resY; 

            // preserve reference to the camera frames to be saved later

            video_frames = c.buffer_items;

            if (video_frames.Count > 0)
            {
                aviManager = new AviManager(fileName,false);                                    // false creates a new file
                stream = aviManager.AddVideoStream(true, frameRate, video_frames[0].image);     // add first frame as an example of the video's format
                
                // add the frames
                ThreadPool.QueueUserWorkItem(new WaitCallback(t_add_frames));         
            }
        }

        /// <summary>
        /// Write out the video in a background thread
        /// </summary>
        /// <param name="state"></param>
        private void t_add_frames(Object state)
        {
            Logger.WriteLine("Adding Frames");
            foreach (BitmapTimestamp img in video_frames)
            {
                stream.AddFrame(img.image);
                frames++;
            }
            aviManager.Close();
            Logger.WriteLine("Created AVI with " + frames + " frames.");
            if (this.OnVideoSaved != null)
            {
                OnVideoSaved(true);
            }
        }     
    }
}
