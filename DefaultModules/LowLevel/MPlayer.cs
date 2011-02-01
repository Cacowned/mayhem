using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Media;
using System.Threading;

namespace DefaultModules.LowLevel
{
    class MPlayer
    {


        //public static MPlayer instance = new MPlayer();

        public MediaPlayer mediaplayer = new MediaPlayer();

        private object locker = new object();

        public static string TAG = "[MPlayer]";

        private bool isPlaying_ = false;

        public bool isPlaying {
            get {
                lock (locker) {
                    return isPlaying_;
                }

            }
            set {
                lock (locker) {
                    isPlaying_ = value;
                }

            }

        }

        public MPlayer() {

            mediaplayer.MediaEnded += new EventHandler(p_MediaEnded);
            mediaplayer.MediaOpened += new EventHandler(p_MediaOpened);
            mediaplayer.MediaFailed += new EventHandler<ExceptionEventArgs>(mediaplayer_MediaFailed);
        }

        void mediaplayer_MediaFailed(object sender, ExceptionEventArgs e) {
            //throw new NotImplementedException();
            Debug.WriteLine(TAG + " Media Playback Failed");
        }

        public void PlayFile(string filePath) {

            isPlaying = true;
            mediaplayer.Open(new Uri(filePath, UriKind.Absolute));
            Debug.WriteLine(TAG + " Starting new media playback " + filePath);
            mediaplayer.Play();
        }

        void p_MediaOpened(object sender, EventArgs e) {
            Debug.WriteLine(TAG + " Media opened");
        }

        void p_MediaEnded(object sender, EventArgs e) {

            Debug.WriteLine(TAG + " Media Ended!");
            isPlaying = false;
            mediaplayer.Stop();
            mediaplayer.Position = TimeSpan.Zero;
        }


    }
}
