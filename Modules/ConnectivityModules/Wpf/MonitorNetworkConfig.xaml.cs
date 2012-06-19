using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Wpf
{    
    public partial class MonitorNetworkConfig : WpfConfiguration
    {       
        public string NetworkName
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return Strings.NetworkName_Title; }
        }

        public MonitorNetworkConfig(string networkName)
        {
            NetworkName = networkName;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            CanSave = true;

            NetworkNameBox.Text = NetworkName;
         
            string errorString = CheckValidityNetworkName();
            textInvalid.Text = errorString;
            
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        public override void OnSave()
        {
            NetworkName = NetworkNameBox.Text;
        }

        private string CheckValidityNetworkName()
        {
            int textLength = NetworkNameBox.Text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
                errorString = Strings.WiFi_NetworkName_NoCharacter;
            else
                if (textLength > 100)
                    errorString = Strings.WiFi_NetworkName_TooLong;

            CanSave = textLength > 0 && (textLength <= 100);

            return errorString;
        }       

        private void NetworkNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string errorString = CheckValidityNetworkName();

            // In the case that we have an error message we display it
            if (!errorString.Equals(string.Empty))
                textInvalid.Text = errorString;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
