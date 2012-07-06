using MayhemWpf.UserControls;

namespace ConnectivityModule.Wpf
{
    public class BTBaseConfig : WpfConfiguration
    {
        /// <summary>
        /// The name of the device
        /// </summary>
        public string DeviceName
        {
            get;
            protected set;
        }

        /// <summary>
        /// The address of the device.
        /// </summary>
        public string DeviceAddress
        {
            get;
            protected set;
        }

        /// <summary>
        /// This method will check if the name of the device is valid.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
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

        /// <summary>
        /// This method will check if the address of the device is valid.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
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
