using GoogleModules.Resources;
using MayhemWpf.UserControls;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// This is the base configuration file for the Youtube configuration files.
    /// </summary>
    public class YouTubeBaseConfig : WpfConfiguration
    {
        public string Username
        {
            get;
            protected set;
        }

        protected string CheckValidityUsername(string value)
        {
            string errorString = string.Empty;

            if (string.IsNullOrEmpty(value))
            {
                CanSave = false;

                return string.Format(Strings.General_NoCharacter, Strings.YouTube_Username);
            }

            int textLength = value.Length;

            if (textLength > 100)
            {
                errorString = string.Format(Strings.General_TooLong, Strings.YouTube_Username);
            }

            CanSave = textLength > 0 && (textLength <= 100);

            return errorString;
        }
    }
}
