using System;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using System.Timers;

namespace KinectModules.Wpf
{
    public partial class KinectSpeechConfig : WpfConfiguration
    {
        public string Phrase
        {
            get;
            private set;
        }

    	private Timer timer;

    	private bool textValid;

        public KinectSpeechConfig(string phrase)
        {
            Phrase = phrase;
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Kinect: Speech Recognition"; }
        }

        public override void OnLoad()
        {
            PhraseTextBox.Text = Phrase;
        	timer = new Timer(500);
			timer.Elapsed += timer_Elapsed;
        	timer.Start();
        }

		public override void OnClosing()
		{
			timer.Stop();
		}

		public void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				if (KinectManager.IsKinectAttached())
				{
					kinectAttached.Visibility = Visibility.Collapsed;

					if (textValid)
					{
						CanSave = true;
					}
				}
				else
				{
					kinectAttached.Visibility = Visibility.Visible;
					CanSave = false;
				}
			}));
		}

        public override void OnSave()
        {
            Phrase = PhraseTextBox.Text.Trim();
        }

        private void CheckValidity()
        {
            textValid = PhraseTextBox.Text.Trim().Length > 0;
        }

        private void PhraseText_TextChanged(object sender, TextChangedEventArgs e)
        {
        	CheckValidity();

			textInvalid.Visibility = textValid ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}