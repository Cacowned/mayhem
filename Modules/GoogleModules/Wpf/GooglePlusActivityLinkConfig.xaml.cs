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

        public GooglePlusActivityLinkConfig(string activityLink, string title)
        {
            ActivityLink = activityLink;
            configTitle = title;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            ActivityLinkBox.Text = ActivityLink;

            CheckValidity();
        }

        public override void OnSave()
        {
            ActivityLink = ActivityLinkBox.Text.Replace("u/0/", "");
        }

        /// <summary>
        /// This method obtains the profile ID by parsing the link of the activity the user chooses to monitor. The profile ID is the 4th element if we split the activity link by the '/' character.
        /// </summary>
        public bool FindProfileID(string activityLink)
        {
            if (string.IsNullOrEmpty(activityLink))
            {
                errorString = string.Format(Strings.General_Incorrect, Strings.General_ActivityLink);
                return false;
            }

            // These characters are not found in the url path so we need to remove them from the link provided by the user.
            ActivityLink = activityLink.Replace("u/0/", "");

            string[] chunks = ActivityLink.Split(new char[] { '/' });

            if (chunks.Length < 4)
            {
                errorString = string.Format(Strings.General_Incorrect, Strings.General_ProfileID);
                return false;
            }

            ProfileID = chunks[3];

            return true;
        }

        protected override void CheckValidity()
        {
            errorString = string.Empty;

            bool evaluation = FindProfileID(ActivityLinkBox.Text) && CheckValidityField(ProfileID, Strings.General_ProfileID, maxLength: 100);
            DisplayErrorMessage(textInvalid);
        }
    }
}
