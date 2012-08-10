using GoogleModules.Resources;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// This is the configuration file for setting the Profile ID of the user you want to monitor.
    /// </summary>
    public partial class GooglePlusProfileIDConfig : GooglePlusBaseConfig
    {
        public GooglePlusProfileIDConfig(string profileID, string title)
        {
            ProfileID = profileID;
            configTitle = title;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            ProfileIDBox.Text = ProfileID;

            CheckValidity();
        }

        public override void OnSave()
        {
            ProfileID = ProfileIDBox.Text;
        }

        protected override void CheckValidity()
        {
            errorString = string.Empty;

            CheckValidityField(ProfileIDBox.Text, Strings.General_ProfileID, maxLength: 100);
            DisplayErrorMessage(textInvalid);
        }
    }
}
