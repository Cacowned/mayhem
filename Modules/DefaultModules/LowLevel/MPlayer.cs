using System;
using System.Diagnostics;
using System.Windows.Media;
using MayhemCore;
using System.Windows.Threading;
using System.Threading;

namespace DefaultModules.LowLevel
{
    class MPlayer
    {
        public MediaPlayer mediaplayer = new MediaPlayer();

        private object locker = new object();

        public static string TAG = "[MPlayer]";

        private bool isPlaying_ = false;

        private Thread runThread = null;

        private Uri fileToPlay = null; 

        public bool isPlaying
        {
            get
            {
                lock (locker)
                {
                    return isPlaying_;
                }

            }
            set
            {
                lock (locker)
                {
                    isPlaying_ = value;
                }

            }

        }

        public MPlayer()
        {
            runThread = new Thread(new ThreadStart(t_runDispatcher));
            runThread.IsBackground = true;
            runThread.Start();
            
        }

        void mediaplayer_MediaFailed(object sender, ExceptionEventArgs e)
        {
            //throw new NotImplementedException();
            Logger.WriteLine(" Media Playback Failed");
        }

        public void PlayFile(string filePath)
        {
            fileToPlay = new Uri(filePath, UriKind.Absolute);
           
            mediaplayer.Dispatcher.BeginInvoke(new Action(delegate() {
                mediaplayer.Open(fileToPlay);
                mediaplayer.Play(); }), null);
        }

        private void t_runDispatcher()
        {
            
            Dispatcher.CurrentDispatcher.VerifyAccess();
            mediaplayer = new MediaPlayer();
            mediaplayer.MediaEnded += new EventHandler(p_MediaEnded);
            mediaplayer.MediaOpened += new EventHandler(p_MediaOpened);
            mediaplayer.MediaFailed += new EventHandler<ExceptionEventArgs>(mediaplayer_MediaFailed);
            Dispatcher.Run();  
        }

        public void Stop()
        {
            Logger.WriteLine("");
            mediaplayer.Dispatcher.BeginInvoke(new Action(delegate(){ mediaplayer.Stop(); }), DispatcherPriority.Send);
        }

        void p_MediaOpened(object sender, EventArgs e)
        {
            Logger.WriteLine(" Media opened");
        }

        void p_MediaEnded(object sender, EventArgs e)
        {

            Logger.WriteLine(" Media Ended!");
            isPlaying = false;
            mediaplayer.Stop();
            mediaplayer.Position = TimeSpan.Zero;
        }


    }
}
