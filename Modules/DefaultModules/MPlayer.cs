using System;
using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;
using MayhemCore;

namespace DefaultModules
{
    class MPlayer
    {
        public MediaPlayer MediaPlayer = new MediaPlayer();
        
        private Thread runThread = null;

        private Uri fileToPlay = null;

        private bool isPlaying = false; 

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
            if (!isPlaying)
            {
                isPlaying = true; 
                runThread.Start();
            }


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
            MediaPlayer = new MediaPlayer();
            MediaPlayer.MediaEnded += new EventHandler(p_MediaEnded);
            MediaPlayer.MediaOpened += new EventHandler(p_MediaOpened);
            MediaPlayer.MediaFailed += new EventHandler<ExceptionEventArgs>(mediaplayer_MediaFailed);
            MediaPlayer.Open(fileToPlay);
            MediaPlayer.Play();
            Dispatcher.Run();


        }

        public void Stop()
        {

            Logger.WriteLine("");

            MediaPlayer.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    MediaPlayer.Stop();
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
            isPlaying = false; 
            Logger.WriteLine(" Media Ended!");
            MediaPlayer.Stop();
            MediaPlayer.Position = TimeSpan.Zero;
        }


    }
}
