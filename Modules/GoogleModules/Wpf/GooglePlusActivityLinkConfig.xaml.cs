using System.Windows;
using System.Windows.Controls;
using GoogleModules.Resources;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// This is the configuration file for setting the link of an activity from the user's Google+ feed.
    /// </summary>
    public partial class GooglePlusActivityLinkConfig : GooglePlusBaseConfig
    {
        public string ActivityLink
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        public GooglePlusActivityLinkConfig(string activityLink, string title)
        {
            ActivityLink = activityLink;
            configTitle = title;

            InitializeComponent();

            FindProfileID(ActivityLink);

            DisplayErrorMessage(CheckValidityProfileID(ProfileID));
        }

        public override void OnLoad()
        {
            CanSave = true;
            ActivityLinkBox.Text = ActivityLink;

            FindProfileID(ActivityLink);

            DisplayErrorMessage(CheckValidityProfileID(ProfileID));
        }

        public override void OnSave()
        {
            ActivityLink = ActivityLinkBox.Text.Replace("u/0/", "");
        }

        /// <summary>
        /// This method obtains the profile ID by parsing the link of the activity the user chooses to monitor. The profile ID is the 4th element if we split the activity link by the '/' character.
        /// </summary>
        public void FindProfileID(string activityLink)
        {
            if (string.IsNullOrEmpty(activityLink))
            {
                return;
            }

            // These characters are not found in the url path so we need to remove them from the link provided by the user.
            ActivityLink = activityLink.Replace("u/0/", "");

            string[] chunks = ActivityLink.Split(new char[] { '/' });

            if (chunks.Length < 4)
            {
                DisplayErrorMessage(string.Format(Strings.General_Incorrect, Strings.General_ProfileID));

                return;
            }

            ProfileID = chunks[3];

            DisplayErrorMessage(CheckValidityProfileID(ProfileID));
        }

        private void ActivityLinkBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Parsing the activity link in order to find the profile ID.
            FindProfileID(ActivityLinkBox.Text);
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
