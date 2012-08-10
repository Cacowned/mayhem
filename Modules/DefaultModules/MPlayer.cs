using System;
using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;
using MayhemCore;

namespace DefaultModules
{
	internal class MPlayer
	{
		private MediaPlayer mediaPlayer;

		private Uri fileToPlay;

		private bool isPlaying;
        
		public MPlayer()
		{
			fileToPlay = null;
			isPlaying = false;
		}

		private void MediaFailed(object sender, ExceptionEventArgs e)
		{
			Logger.WriteLine("Media Playback Failed");
		}

		public void PlayFile(string filePath)
		{
			fileToPlay = new Uri(filePath, UriKind.Absolute);

            // If called while playing, it will ignore until media ends
            if (!isPlaying)
			{         
                
                isPlaying = true;                
                Thread newThread = new Thread(StartPlaying);
                newThread.IsBackground = true;
                newThread.Start();
            }   
        }		

		private void StartPlaying()
		{
			Dispatcher.CurrentDispatcher.VerifyAccess();
			mediaPlayer = new MediaPlayer();
			mediaPlayer.MediaEnded += MediaEnded;
			mediaPlayer.MediaOpened += MediaOpened;
			mediaPlayer.MediaFailed += MediaFailed;
			mediaPlayer.Open(fileToPlay);
			mediaPlayer.Play();
			Dispatcher.Run();
		}

		public void Stop()
		{
			mediaPlayer.Dispatcher.BeginInvoke
			(
				new Action(() => mediaPlayer.Stop()),
				DispatcherPriority.Send
			);            
		}

		private void MediaOpened(object sender, EventArgs e)
		{
			Logger.WriteLine("Media Opened");
		}

		private void MediaEnded(object sender, EventArgs e)
		{
			isPlaying = false;
			Logger.WriteLine("Media Ended!");
			mediaPlayer.Stop();
			mediaPlayer.Position = TimeSpan.Zero;
		}
	}
}
