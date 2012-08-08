using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using SkypeModules.Resources;

namespace SkypeModules.Wpf
{
    /// <summary>
    /// Base Class for defining the common method and members.
    /// </summary>
    public class SkypeBaseConfig : WpfConfiguration
    {
        public string SkypeID
        {
            get;
            protected set;
        }

        public override string Title
        {
            get { return configTitle; }
        }

        protected string configTitle;
        protected string errorString;

        protected bool CheckValidityField(string text, string type, int maxLength)
        {
            errorString = string.Empty;
            int textLength = text.Length;

            if (textLength == 0)
            {
                errorString = string.Format(Strings.General_NoCharacter, type);
            }
            else if (textLength > maxLength)
            {
                errorString = string.Format(Strings.General_TooLong, type);
            }

            return errorString == string.Empty;
        }

        protected void DisplayErrorMessage(TextBlock textInvalid)
        {
            CanSave = true;

            // In the case that we have an error message we display it.
            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                CanSave = false;
            }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        protected void Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        protected virtual void CheckValidity()
        {
            // This method will be overriden in the classes that need to check the validity of it's fields. 
            // It is needed to be declared here so I can call it in the Box_TextChanged method.
        }
    }
}
