using System.Windows;
using System.Windows.Controls;
using GoogleModules.Resources;
using MayhemWpf.UserControls;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// User Control for setting the information about the moment that needs to be added
    /// </summary>
    public partial class GooglePlusAddMomentConfig : WpfConfiguration
    {
        /// <summary>
        /// The text that represents the moment
        /// </summary>
        public string MomentText
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
        /// The constructor of the GooglePlusAddMomentConfig class.
        /// </summary>
        /// <param name="profileID">The text that represents the moment</param>
        /// <param name="title">The title of the user control</param>
        /// <param name="momentDetailsText">The text that will be displayed on the user control for additional details</param>
        /// <param name="momentTypeText">The type of the moment that is added</param>
        public GooglePlusAddMomentConfig(string momentText, string title, string momentDetailsText, string momentTypeText)
        {           
            InitializeComponent();

            MomentText = momentText;
            configTitle = title;
            this.DetailsText.Text = momentDetailsText;
            this.TypeText.Text = momentTypeText;
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
        public override void OnLoad()
        {
            CanSave = true;

            ActivityTextBox.Text = MomentText;
            
            DisplayErrorMessage(CheckValidityMomentText());
        }

        /// <summary>
        /// This method will be called when the used clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            MomentText = ActivityTextBox.Text;
        }

        /// <summary>
        /// This method will check if the text of the moment is valid.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
        private string CheckValidityMomentText()
        {
            int textLength = ActivityTextBox.Text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
            {
                errorString = Strings.GooglePlus_MomentText_NoCharacter;
            }
            else
            {
                if (textLength > 300)
                {
                    errorString = Strings.GooglePlus_MomentText_TooLong;
                }
            }

            CanSave = textLength > 0 && (textLength <= 300);

            return errorString;
        }

        /// <summary>
        /// This method will be called when the text from the ActivityTextBox changes.
        /// </summary>
        private void ActivityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayErrorMessage(CheckValidityMomentText());
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
