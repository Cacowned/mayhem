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

namespace DefaultModules.Wpf
{
    public partial class TimerConfig : Window
    {
        public int hours, minutes, seconds;

        public TimerConfig(int hours, int minutes, int seconds) {
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;

            InitializeComponent();

            // I should be using bindings instead of setting this directly
            HoursBox.Text = hours.ToString();
            MinutesBox.Text = minutes.ToString();
            SecondsBox.Text = seconds.ToString();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            bool badsec,badmin,badhour;

            badsec = !(Int32.TryParse(SecondsBox.Text, out seconds) && (seconds >= 0 && seconds < 60));

            badmin = !(Int32.TryParse(MinutesBox.Text, out minutes) && (minutes >= 0 && minutes < 60));

            badhour = !(Int32.TryParse(HoursBox.Text, out hours) && (hours >= 0));

            StringBuilder s = new StringBuilder();
            s.Append("Invalid");

            if (badsec)
            {
                s.Append(" seconds");
            }
            if (badmin)
            {
                s.Append(" minutes");
            }
            if (badhour)
            {
                s.Append(" hours");
            }

            if (badsec || badmin || badhour)
            {
                MessageBox.Show(s.ToString());
                return;
            }

            DialogResult = true;
        }
    }
}
