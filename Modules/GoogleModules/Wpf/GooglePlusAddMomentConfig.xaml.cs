using System.Windows;
using System.Windows.Controls;
using GoogleModules.Resources;
using MayhemWpf.UserControls;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// User Control for setting the information about the moment that needs to be added.
    /// </summary>
    public partial class GooglePlusAddMomentConfig : WpfConfiguration
    {
        public string MomentText
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        public GooglePlusAddMomentConfig(string momentText, string title, string momentDetailsText, string momentTypeText)
        {           
            InitializeComponent();

            MomentText = momentText;
            configTitle = title;
            this.DetailsText.Text = momentDetailsText;
            this.TypeText.Text = momentTypeText;
        }

        public override void OnLoad()
        {
            CanSave = true;

            ActivityTextBox.Text = MomentText;
            
            DisplayErrorMessage(CheckValidityMomentText());
        }

        public override void OnSave()
        {
            MomentText = ActivityTextBox.Text;
        }

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

        private void ActivityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayErrorMessage(CheckValidityMomentText());
        }

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
