using System;
using System.Text;
using System.Windows;
using MayhemDefaultStyles.UserControls;

namespace DefaultModules.Wpf
{
	public partial class TimerConfig : IWpfConfig
	{
		public int Hours, Minutes, Seconds;

		public TimerConfig(int hours, int minutes, int seconds) {
			this.Hours = hours;
			this.Minutes = minutes;
			this.Seconds = seconds;

			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			HoursBox.Text = Hours.ToString();
			MinutesBox.Text = Minutes.ToString();
			SecondsBox.Text = Seconds.ToString();
		}

        public override string Title
        {
            get { return "Timer"; }
        }

        public override bool OnSave()
        {
            // TODO: Change this error checking to be on text changed
            bool badsec, badmin, badhour;

            badsec = !(Int32.TryParse(SecondsBox.Text, out Seconds) && (Seconds >= 0 && Seconds < 60));

            badmin = !(Int32.TryParse(MinutesBox.Text, out Minutes) && (Minutes >= 0 && Minutes < 60));

            badhour = !(Int32.TryParse(HoursBox.Text, out Hours) && (Hours >= 0));

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
                return false;
            }
            return true;
        }

        public override void OnCancel()
        {
        }
	}
}
