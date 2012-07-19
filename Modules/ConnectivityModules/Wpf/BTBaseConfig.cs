using MayhemWpf.UserControls;

namespace ConnectivityModule.Wpf
{
    public class BTBaseConfig : WpfConfiguration
    {
        public string DeviceName
        {
            get;
            protected set;
        }

        public string DeviceAddress
        {
            get;
            protected set;
        }

        protected string CheckValidityDeviceName(string name)
        {
            int textLength = name.Length;
            string errorString = string.Empty;

            if (textLength == 0)
            {
                errorString = Strings.BT_DeviceName_NoCharacter;
            }
            else
            {
                if (textLength > 20)
                {
                    errorString = Strings.BT_DeviceName_TooLong;
                }
            }

            CanSave = textLength != 0 && textLength <= 20;

            return errorString;
        }

        protected string CheckValidityDeviceAddress(string deviceAddress)
        {
            int textLength = deviceAddress.Length;
            string errorString = string.Empty;

            if (textLength < 12)
            {
                errorString = Strings.BT_DeviceAddress_TooShort;
            }
            else
            {
                if (textLength > 17)
                {
                    errorString = Strings.BT_DeviceAddress_TooLong;
                }
            }

            CanSave = textLength >= 12 && textLength <= 17;

            return errorString;
        }
    }
}
