using System;
using System.Text;
using System.Windows;

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
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			HoursBox.Text = hours.ToString();
			MinutesBox.Text = minutes.ToString();
			SecondsBox.Text = seconds.ToString();
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}

		private void Button_Save_Click(object sender, RoutedEventArgs e) {
			bool badsec, badmin, badhour;

			badsec = !(Int32.TryParse(SecondsBox.Text, out seconds) && (seconds >= 0 && seconds < 60));

			badmin = !(Int32.TryParse(MinutesBox.Text, out minutes) && (minutes >= 0 && minutes < 60));

			badhour = !(Int32.TryParse(HoursBox.Text, out hours) && (hours >= 0));

			StringBuilder s = new StringBuilder();
			s.Append("Invalid");

			if (badsec) {
				s.Append(" seconds");
			}
			if (badmin) {
				s.Append(" minutes");
			}
			if (badhour) {
				s.Append(" hours");
			}

			if (badsec || badmin || badhour) {
				MessageBox.Show(s.ToString());
				return;
			}

			DialogResult = true;
		}
	}
}
