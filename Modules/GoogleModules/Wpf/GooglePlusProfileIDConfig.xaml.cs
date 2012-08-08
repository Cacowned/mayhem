using System.Windows;
using System.Windows.Controls;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// This is the configuration file for setting the Profile ID of the user you want to monitor.
    /// </summary>
    public partial class GooglePlusProfileIDConfig : GooglePlusBaseConfig
    {
        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        public GooglePlusProfileIDConfig(string profileID, string title)
        {
            ProfileID = profileID;
            configTitle = title;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            CanSave = true;
            ProfileIDBox.Text = ProfileID;

            DisplayErrorMessage(CheckValidityProfileID(ProfileID));
        }

        public override void OnSave()
        {
            ProfileID = ProfileIDBox.Text;
        }

        private void ProfileIDBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayErrorMessage(CheckValidityProfileID(ProfileIDBox.Text));
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
