using System.Windows;
using System.Windows.Controls;
using GoogleModules.Resources;
using MayhemWpf.UserControls;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// User Control for setting the Profile ID of the user you want to monitor.
    /// </summary>
    public partial class GooglePlusProfileIDConfig : WpfConfiguration
    {
        /// <summary>
        /// The Profile ID.
        /// </summary>
        public string ProfileID
        {
            get;
            private set;
        }

        /// <summary>
        /// The title of the user control.
        /// </summary>
        public override string Title
        {
            get { return configTitle; }
        }


        private string configTitle;

        /// <summary>
        /// The constructor of the GooglePlusProfileIDConfig class.
        /// </summary>
        /// <param name="profileID">The Profile ID</param>
        public GooglePlusProfileIDConfig(string profileID, string title)
        {
            ProfileID = profileID;
            configTitle = title;

            InitializeComponent();
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
        public override void OnLoad()
        {
            CanSave = true;

            ProfileIDBox.Text = ProfileID;

            DisplayErrorMessage(CheckValidityProfileID());
        }

        /// <summary>
        /// This method will be called when the used clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            ProfileID = ProfileIDBox.Text;
        }

        /// <summary>
        /// This method will check if the Profile ID is valid.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
        private string CheckValidityProfileID()
        {
            int textLength = ProfileIDBox.Text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
            {
                errorString = Strings.GooglePlus_ProfileID_NoCharacter;
            }
            else
            {
                if (textLength > 100)
                {
                    errorString = Strings.GooglePlus_ProfileID_TooLong;
                }
            }

            CanSave = textLength > 0 && (textLength <= 100);

            return errorString;
        }

        /// <summary>
        /// This method will be called when the text from the ProfileIDBox changes.
        /// </summary>
        private void ProfileIDBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayErrorMessage(CheckValidityProfileID());
        }

        /// <summary>
        /// Displays the error message received as parameter.
        /// </summary>
        /// <param name="errorMessage">The text of the error message</param>
        private void DisplayErrorMessage(string errorString)
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
