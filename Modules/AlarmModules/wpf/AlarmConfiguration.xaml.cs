using System;
using System.Globalization;
using System.Windows;
using MayhemWpf.UserControls;
using System.Windows.Controls;

namespace AlarmModules
{
    /// <summary>
    /// Interaction logic for AlarmConfiguration.xaml
    /// </summary>
    public partial class AlarmConfig : WpfConfiguration
    {
        public bool RecurDaily { get; private set; }
        public int Day { get; private set; }
        public int Month { get; private set; }
        public int Year { get; private set; }
        public int Hour { get; private set; }
        public int Minute { get; private set; }
        public int Second { get; private set; }

        private bool shouldCheckValidity;

        public AlarmConfig(bool recurs, int day, int month, int year, int hour, int minute, int second)
        {
            RecurDaily = recurs;
            Day = (day == 0) ? DateTime.Now.Day : day;
            Month = (month == 0) ? DateTime.Now.Month : month;
            Year = (year == 0) ? DateTime.Now.Year : year;
            Hour = hour;
            Minute = minute;
            Second = second;
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Alarm"; }
        }

        public override void OnLoad()
        {
            RecurringDailyCheckBox.IsChecked = RecurDaily;
            HourBox.Text = Hour.ToString();
            MinuteBox.Text = Minute.ToString();
            SecondBox.Text = Second.ToString();
            DateTime dtToSelect = new DateTime(Year, Month, Day);
            AlarmDatePick.SelectedDate = dtToSelect;

            if (dtToSelect.CompareTo(DateTime.Now) > 0)
            {
                AlarmDatePick.BlackoutDates.Add(
                    new CalendarDateRange(DateTime.MinValue, DateTime.Now.Subtract(
                        new TimeSpan(1, 0, 0, 0))));
            }
            else
            {
                AlarmDatePick.BlackoutDates.Add(new CalendarDateRange(
                    DateTime.MinValue, dtToSelect.Subtract(
                        new TimeSpan(1, 0, 0, 0))));
            }
           
            shouldCheckValidity = true;
        }

        public override void OnSave()
        {
            Second = int.Parse(SecondBox.Text);
            Minute = int.Parse(MinuteBox.Text);
            Hour = int.Parse(HourBox.Text);
            DateTime finalDate = AlarmDatePick.SelectedDate.GetValueOrDefault();
            Day = finalDate.Day;
            Month = finalDate.Month;
            Year = finalDate.Year;
            RecurDaily = (RecurringDailyCheckBox.IsChecked == true) ? true : false;
        }

        private string CheckValidity()
        {
            int seconds, minutes, hours, day, month, year;
            DateTime alarmTime;
            string errorString = "Invalid";
            bool badsec = !(int.TryParse(SecondBox.Text, out seconds) && (seconds >= 0 && seconds < 60));
            bool badmin = !(int.TryParse(MinuteBox.Text, out minutes) && (minutes >= 0 && minutes < 60));
            bool badhour = !(int.TryParse(HourBox.Text, out hours) && (hours >= 0 && hours < 24));
            bool badday = false;
            bool badmonth = false;
            bool badyear = false;
            bool badtotal = false;
            DateTime finalDate = AlarmDatePick.SelectedDate.GetValueOrDefault();
            day = finalDate.Day;
            month = finalDate.Month;
            year = finalDate.Year;
            bool badDateTime = !(badsec || badmin || badhour || badday || badmonth || badyear);

            if (RecurringDailyCheckBox.IsChecked == false)
            {
                if (badDateTime)
                {
                    string time = string.Format("{0}/{1}/{2} {3}:{4}:{5}", month, day, year, hours, minutes, seconds);
                    badtotal = !(DateTime.TryParseExact(time, "M/d/yyyy H:m:s", CultureInfo.InvariantCulture, DateTimeStyles.None, out alarmTime) && (alarmTime.CompareTo(DateTime.Now)) > 0);
                }
            }

            if (badsec)
                errorString += " seconds";
            if (badmin)
                errorString += " minutes";
            if (badhour)
                errorString += " hours";
            if (badday)
                errorString += " days";
            if (badmonth)
                errorString += " month";
            if (badyear)
                errorString += " year";
            if (badtotal && badDateTime)
                errorString = "Must be a valid Date and Time in the Future";
            CanSave = !(badsec || badmin || badhour || badday || badmonth || badyear || badtotal);
            return CanSave ? string.Empty : errorString;
        }

        private void TextChanged(object sender, EventArgs e)
        {
            if (RecurringDailyCheckBox.IsChecked == true)
            {
                AlarmDatePick.IsEnabled = false;
            }
            else
            {
                AlarmDatePick.IsEnabled = true;
            }

            textInvalid.Text = CheckValidity();

            if (shouldCheckValidity)
            {
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
