using MayhemWpf.UserControls;

namespace ConnectivityModule.Wpf
{
    public class BTPairConfig : BTBaseConfig
    {
        /// <summary>
        /// The pin used for pairing with the device.
        /// </summary>
        public string Pin
        {
            get;
            protected set;
        }

        /// <summary>
        /// This method will check if the pin is valid.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
        protected string CheckValidityPin(string pinText)
        {
            int textLength = pinText.Length;
            string errorString = string.Empty;

            if (textLength == 0)
            {
                errorString = Strings.BT_Pin_NoCharacter;
            }
            else
                if (textLength > 10)
                {
                    errorString = Strings.BT_Pin_TooLong;
                }

            CanSave = textLength != 0 && textLength <= 10;

            return errorString;
        }
    }
}
