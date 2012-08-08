using System.Windows;
using System.Windows.Controls;
using GoogleModules.Resources;
using MayhemWpf.UserControls;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// This is the configuration file for setting the ID of the YouTube video we want to monitor.
    /// </summary>
    public partial class YouTubeCommentAddedConfig : WpfConfiguration
    {
        public string VideoID
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        public YouTubeCommentAddedConfig(string videoID, string title)
        {
            VideoID = videoID;
            configTitle = title;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            CanSave = true;

            VideoIDBox.Text = VideoID;

            DisplayErrorMessage(CheckValidityVideoID(VideoID));
        }

        public override void OnSave()
        {
            VideoID = VideoIDBox.Text;
        }

        private void VideoIDBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayErrorMessage(CheckValidityVideoID(VideoIDBox.Text));
        }

        protected string CheckValidityVideoID(string value)
        {
            string errorString = string.Empty;

            if (string.IsNullOrEmpty(value))
            {
                CanSave = false;

                return string.Format(Strings.General_NoCharacter, Strings.YouTube_VideoID);
            }

            int textLength = value.Length;

            if (textLength > 100)
            {
                errorString = string.Format(Strings.General_TooLong, Strings.YouTube_VideoID);
            }

            CanSave = textLength > 0 && (textLength <= 100);

            return errorString;
        }

        protected void DisplayErrorMessage(string errorString)
        {
            // In the case that we have an error message we display it.
            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
            }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
