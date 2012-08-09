using GoogleModules.Resources;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// This is the configuration file for setting the username for the feed we want to monitor.
    /// </summary>
    public partial class YouTubeUsernameConfig : GoogleBaseConfig
    {
        public string Username
        {
            get;
            protected set;
        }

        public YouTubeUsernameConfig(string username, string title)
        {
            Username = username;
            configTitle = title;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            UsernameBox.Text = Username;

            CheckValidity();
        }

        public override void OnSave()
        {
            Username = UsernameBox.Text;
        }

        protected override void CheckValidity()
        {
            errorString = string.Empty;

            CheckValidityField(UsernameBox.Text, Strings.YouTube_Username, maxLength: 100);
            DisplayErrorMessage(textInvalid);
        }
    }
}
