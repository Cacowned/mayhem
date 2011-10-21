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
        }

        public override void OnSave()
        {
            Seconds = int.Parse(SecondsBox.Text);
            Minutes = int.Parse(MinutesBox.Text);
            Hours = int.Parse(HoursBox.Text);
        }

        private string CheckValidity()
        {
            int seconds, minutes, hours;

            bool badsec = !(int.TryParse(SecondsBox.Text, out seconds) && (seconds >= 0 && seconds < 60));
            bool badmin = !(int.TryParse(MinutesBox.Text, out minutes) && (minutes >= 0 && minutes < 60));
            bool badhour = !(int.TryParse(HoursBox.Text, out hours) && (hours >= 0));

            bool badtotal = seconds == 0 && minutes == 0 && hours == 0;

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

            CanSave = !(badsec || badmin || badhour || badtotal);
            return CanSave ? string.Empty : s;
        }

        private void TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            textInvalid.Text = CheckValidity();
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
