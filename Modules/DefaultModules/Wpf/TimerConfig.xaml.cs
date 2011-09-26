using System;
using System.Windows;
using MayhemWpf.UserControls;

namespace DefaultModules.Wpf
{
    public partial class TimerConfig : WpfConfiguration
    {
        public int Hours
        {
            get;
            private set;
        }
        public int Minutes
        {
            get;
            private set;
        }
        public int Seconds
        {
            get;
            private set;
        }

        private bool shouldCheckValidity = false;

        public TimerConfig(int hours, int minutes, int seconds)
        {
            this.Hours = hours;
            this.Minutes = minutes;
            this.Seconds = seconds;

            InitializeComponent();
        }

        public override string Title
        {
            get { return "Timer"; }
        }

        public override void OnLoad()
        {
            HoursBox.Text = Hours.ToString();
            MinutesBox.Text = Minutes.ToString();
            SecondsBox.Text = Seconds.ToString();

            shouldCheckValidity = true;
        }

        private string CheckValidity()
        {
            bool badsec, badmin, badhour, badtotal;

            int sec, min, hour;

            badsec = !(Int32.TryParse(SecondsBox.Text, out sec) && (Seconds >= 0 && Seconds < 60));

            badmin = !(Int32.TryParse(MinutesBox.Text, out min) && (Minutes >= 0 && Minutes < 60));

            badhour = !(Int32.TryParse(HoursBox.Text, out hour) && (Hours >= 0));

            badtotal = Seconds == 0 && Minutes == 0 && Hours == 0;

            string s = "Invalid";

            if (badsec)
            {
                s += " seconds";
            }
            if (badmin)
            {
                s += " minutes";
            }
            if (badhour)
            {
                s += " hours";
            }
            if (badtotal && !(badsec || badmin || badhour))
                s = "Timer length must be greater than 0.";

            if (badsec || badmin || badhour || badtotal)
            {
                CanSave = false;
                return s;
            }
            else
            {
                Seconds = sec;
                Minutes = min;
                Hours = hour;

                CanSave = true;
                return string.Empty;
            }
        }

        private void TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (shouldCheckValidity)
            {
                textInvalid.Text = CheckValidity();
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
