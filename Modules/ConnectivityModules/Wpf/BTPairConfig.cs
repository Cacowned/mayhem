using MayhemWpf.UserControls;

namespace ConnectivityModule.Wpf
{
    public class BTPairConfig : BTBaseConfig
    {
        public string Pin
        {
            get;
            protected set;
        }

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
