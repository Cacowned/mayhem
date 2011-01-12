using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Media;
using System.Threading;

namespace MayhemApp.Low_Level
{
    class MPlayer
    {


        //public static MPlayer instance = new MPlayer();

        public MediaPlayer mediaplayer = new MediaPlayer();

        private object locker = new object();

        public static string TAG = "[MPlayer] : ";

        private bool isPlaying_ = false;

        public bool isPlaying 
        {
            get 
            {
                lock(locker)
                {
                    return isPlaying_;
                }

            }
            set 
            {
                lock(locker)
                {
                    isPlaying_ = value;
                }

            }
            
        }

        // declare the delegate
        public delegate bool AudioCallbackDelegate(IntPtr hwnd, int lParam);

        public MPlayer()
        {
           
            mediaplayer.MediaEnded += new EventHandler(p_MediaEnded);
            mediaplayer.MediaOpened += new EventHandler(p_MediaOpened);
            mediaplayer.MediaFailed += new EventHandler<ExceptionEventArgs>(mediaplayer_MediaFailed);
        }

        void mediaplayer_MediaFailed(object sender, ExceptionEventArgs e)
        {
            //throw new NotImplementedException();
            Debug.WriteLine(TAG + "media Playback Failed");
        }

        public void PlayFile(string filePath)
        {

            Debug.WriteLine(TAG + "Playing Test File");

            isPlaying = true;
            mediaplayer.Open(new Uri(filePath, UriKind.Absolute));
            Debug.WriteLine(TAG + "starting new media playback " + filePath);
            mediaplayer.Play();
        }

        void p_MediaOpened(object sender, EventArgs e)
        {
            Debug.WriteLine(TAG + "media opened");
        }

        void p_MediaEnded(object sender, EventArgs e)
        {
           
            Debug.WriteLine("Media Ended!");
            isPlaying = false;
            mediaplayer.Stop();
            mediaplayer.Position = TimeSpan.Zero;
        }



#region Testing
        public void TestPlayFile()
        {
            string path = "G:\\bullet_ricochet.mp3";
            PlayFile(path);
        }
#endregion


    }
}
