using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using DisplayWindowModule.Resources;
using MayhemWpf.UserControls;

namespace DisplayWindowModuleWpf.Wpf
{
    public partial class DisplayWindowConfig : WpfConfiguration
    {
        public string Message
        {
            get;
            private set;
        }

        public int Seconds
        {
            get;
            private set;
        }

        public DisplayWindowConfig(string message, int seconds)
        {
            Message = message;
            Seconds = seconds;
            InitializeComponent();
        }

        public override string Title
        {
            get
            {
                return Strings.DisplayWindow_Title;
            }
        }

        public override void OnLoad()
        {
            CanSave = true;

            MessageBox.Text = Message;

            //the minimum time span must be 1
            if (Seconds == 0)
                Seconds = 1;

            SecondsBox.Text = Seconds.ToString(CultureInfo.InvariantCulture);

            //we need to check if the message and the number of seconds are setted correctly
            string errorString = CheckValidityMessage();
            
            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                textInvalid.Visibility = Visibility.Visible;
                return;
            }
            
            errorString = CheckValiditySeconds();

            if (!errorString.Equals(string.Empty))
                textInvalid.Text = errorString;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        public override void OnSave()
        {
            Message = MessageBox.Text;
            Seconds = int.Parse(SecondsBox.Text);
        }

        private string CheckValidityMessage()
        {
            int textLength = MessageBox.Text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
                errorString = Strings.DisplayWindow_Message_NoCharacter;
            else
                if (textLength > 100)
                    errorString = Strings.DisplayWindow_Message_TooLong;

            CanSave = textLength > 0 && (textLength <= 100);

            return errorString;
        }

        private string CheckValiditySeconds()
        {
            int seconds;
            string errorString = string.Empty;

            bool badsec = !(int.TryParse(SecondsBox.Text, out seconds) && (seconds >= 0 && seconds < 60));

            if (badsec)
                errorString = Strings.DisplayWindow_SecondsInvalid;
            else
                if (seconds == 0)
                    errorString = Strings.DisplayWindow_Seconds_GreaterThanZero;

            CanSave = !badsec && seconds != 0;

            return errorString;
        }

        private void MessageText_TextChanged(object sender, TextChangedEventArgs e)
        {
            string errorString = CheckValidityMessage();
           
            //in the case that we have an error message we display it
            if (!errorString.Equals(string.Empty))
                textInvalid.Text = errorString;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SecondsBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string errorString = CheckValiditySeconds();
         
            //in the case that we have an error message we display it
            if (!errorString.Equals(string.Empty))
                textInvalid.Text = errorString;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
