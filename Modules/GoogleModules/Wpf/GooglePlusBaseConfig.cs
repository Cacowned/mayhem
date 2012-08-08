using GoogleModules.Resources;
using MayhemWpf.UserControls;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// This is the base configuration file for the Google+ configuration files.
    /// </summary>
    public class GooglePlusBaseConfig : WpfConfiguration
    {
        public string ProfileID
        {
            get;
            protected set;
        }

        protected string CheckValidityProfileID(string profileID)
        {
            string errorString = string.Empty;

            if (string.IsNullOrEmpty(profileID))
            {
                CanSave = false;

                return string.Format(Strings.General_NoCharacter, Strings.General_ProfileID);
            }

            int textLength = profileID.Length;

            if (textLength > 100)
            {
                errorString = string.Format(Strings.General_TooLong, Strings.General_ProfileID);
            }

            CanSave = textLength > 0 && (textLength <= 100);

            return errorString;
        }
    }
}
