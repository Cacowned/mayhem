using GoogleModules.Resources;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// This is the configuration file for setting the ID of the YouTube video we want to monitor.
    /// </summary>
    public partial class YouTubeCommentAddedConfig : GoogleBaseConfig
    {
        public string VideoID
        {
            get;
            private set;
        }

        public YouTubeCommentAddedConfig(string videoID, string title)
        {
            VideoID = videoID;
            configTitle = title;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            VideoIDBox.Text = VideoID;

            CheckValidity();
        }

        public override void OnSave()
        {
            VideoID = VideoIDBox.Text;
        }

        protected override void CheckValidity()
        {
            errorString = string.Empty;

            CheckValidityField(VideoIDBox.Text, Strings.YouTube_VideoID, maxLength: 100);
            DisplayErrorMessage(textInvalid);
        }
    }
}
