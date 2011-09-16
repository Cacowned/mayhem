using System;
using System.Diagnostics;
using System.Windows.Media;
using MayhemCore;
using System.Windows.Threading;
using System.Threading;

namespace DefaultModules
{
    class MPlayer
    {
        public MediaPlayer mediaplayer = new MediaPlayer();
        
        private Thread runThread = null;

        private Uri fileToPlay = null;

        public MPlayer()
        {
            runThread = new Thread(new ThreadStart(t_StartPlaying));
            runThread.IsBackground = true;

        }

        void mediaplayer_MediaFailed(object sender, ExceptionEventArgs e)
        {
            //throw new NotImplementedException();
            Logger.WriteLine(" Media Playback Failed");
        }

        public void PlayFile(string filePath)
        {
            fileToPlay = new Uri(filePath, UriKind.Absolute);
            runThread.Start();


            /*
            mediaplayer.Dispatcher.Invoke(new Action(delegate()
                {
                    mediaplayer.Open(new Uri(filePath, UriKind.Absolute));
                    Logger.WriteLine(" Starting new media playback " + filePath);
                    mediaplayer.Play();
                }));
           */
        }

        private void t_StartPlaying()
        {
            Dispatcher.CurrentDispatcher.VerifyAccess();
            mediaplayer = new MediaPlayer();
            mediaplayer.MediaEnded += new EventHandler(p_MediaEnded);
            mediaplayer.MediaOpened += new EventHandler(p_MediaOpened);
            mediaplayer.MediaFailed += new EventHandler<ExceptionEventArgs>(mediaplayer_MediaFailed);
            mediaplayer.Open(fileToPlay);
            mediaplayer.Play();
            Dispatcher.Run();


        }

        public void Stop()
        {
            Logger.WriteLine("");
            mediaplayer.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    mediaplayer.Stop();
                }),
                DispatcherPriority.Send
            );
        }

        void p_MediaOpened(object sender, EventArgs e)
        {
            Logger.WriteLine(" Media opened");
        }

        void p_MediaEnded(object sender, EventArgs e)
        {

            Logger.WriteLine(" Media Ended!");
            mediaplayer.Stop();
            mediaplayer.Position = TimeSpan.Zero;
        }


    }
}
