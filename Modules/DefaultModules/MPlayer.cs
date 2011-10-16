using System;
using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;
using MayhemCore;

namespace DefaultModules
{
    internal class MPlayer
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

        private void mediaplayer_MediaFailed(object sender, ExceptionEventArgs e)
        {
            Logger.WriteLine("Media Playback Failed");
        }

        public void PlayFile(string filePath)
        {
            fileToPlay = new Uri(filePath, UriKind.Absolute);
            if (!isPlaying)
            {
                isPlaying = true;
                runThread.Start();
            }
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
            MediaPlayer.Dispatcher.BeginInvoke(new Action(delegate
            {
                MediaPlayer.Stop();
            }),
                DispatcherPriority.Send);
        }

        private void p_MediaOpened(object sender, EventArgs e)
        {
            Logger.WriteLine("Media opened");
        }

        private void p_MediaEnded(object sender, EventArgs e)
        {
            isPlaying = false;
            Logger.WriteLine("Media Ended!");
            MediaPlayer.Stop();
            MediaPlayer.Position = TimeSpan.Zero;
        }
    }
}
