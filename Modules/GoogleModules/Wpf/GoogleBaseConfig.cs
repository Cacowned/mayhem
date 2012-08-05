using System.Windows;
using System.Windows.Controls;
using GoogleModules.Resources;
using MayhemWpf.UserControls;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// The base clase for the Google configuration user controls.
    /// </summary>
    public class GoogleBaseConfig : WpfConfiguration
    {
        public override string Title
        {
            get { return configTitle; }
        }

        protected string configTitle;
        protected string errorString;

        protected bool CheckValidityField(string text, int maxLength, string type)
        {
            int textLength = text.Length;
            errorString = string.Empty;

            if (textLength == 0)
            {
                errorString = string.Format(Strings.General_NoCharacter, type);
            }
            else if (textLength > maxLength)
            {
                errorString = string.Format(Strings.General_TooLong, type);
            }

            return textLength > 0 && (textLength <= maxLength);
        }

        protected void DisplayErrorMessage(TextBlock textInvalid)
        {
            CanSave = true;

            // In the case that we have an error message we display it.
            if (!errorString.Equals(string.Empty))
            {
                CanSave = false;
                textInvalid.Text = errorString;
            }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
