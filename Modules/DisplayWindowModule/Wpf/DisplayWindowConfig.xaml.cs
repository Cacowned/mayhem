using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MayhemWpf.UserControls;
using System.Globalization;
using DisplayWindowModule.Resources;

namespace DisplayWindowModuleWpf.Wpf
{
    public partial class DisplayWindowConfig : WpfConfiguration
    {
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

        public override void OnLoad()
        {
            CanSave = true;

            MessageBox.Text = Message;

            //the minimum time span must be 1
            if (Seconds == 0)
                Seconds = 1;

            SecondsBox.Text = Seconds.ToString(CultureInfo.InvariantCulture);

            CheckValidityMessage();
            CheckValiditySeconds();
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
