using System.Windows;
using System.Windows.Controls;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// This is the configuration file for setting the username for the feed we want to monitor.
    /// </summary>
    public partial class YoutubeUsernameConfig : YoutubeBaseConfig
    {
        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        public YoutubeUsernameConfig(string username, string title)
        {
            Username = username;
            configTitle = title;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            CanSave = true;

            UsernameBox.Text = Username;

            DisplayErrorMessage(CheckValidityUsername(Username));
        }

        public override void OnSave()
        {
            Username = UsernameBox.Text;
        }

        private void UsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayErrorMessage(CheckValidityUsername(UsernameBox.Text));
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
