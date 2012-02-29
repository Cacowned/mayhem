using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace KinectModules.Wpf
{
    public partial class KinectSpeechConfig : WpfConfiguration
    {
        public string Phrase
        {
            get;
            private set;
        }

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
        }

        public override void OnSave()
        {
            Phrase = PhraseTextBox.Text.Trim();
        }

        private void CheckValidity()
        {
            CanSave = PhraseTextBox.Text.Trim().Length > 0;
        }

        private void PhraseText_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}