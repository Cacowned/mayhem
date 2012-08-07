﻿using System;
using System.Globalization;
using System.Windows;
using MayhemWpf.UserControls;
using System.Windows.Controls;

namespace PreGsocTest1
{
    /// <summary>
    /// Interaction logic for AlarmConfiguration.xaml
    /// </summary>
    public partial class DPAlarmConfig : WpfConfiguration
    {
        public bool RecurDaily { get; private set; }
        public int Day { get; private set; }
        public int Month { get; private set; }
        public int Year { get; private set; }
        public int Hour { get; private set; }
        public int Minute { get; private set; }
        public int Second { get; private set; }
        private bool shouldCheckValidity;
        public DPAlarmConfig(bool recurs, int day, int month, int year, int hour, int minute, int second)
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
                AlarmDatePick.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0))));
            }
            else
            {
                AlarmDatePick.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, dtToSelect.Subtract(new TimeSpan(1, 0, 0, 0))));
            }
            
            //DayBox.Text = Day.ToString();
            //MonthBox.Text = Month.ToString();
            //YearBox.Text = Year.ToString();
            shouldCheckValidity = true;
        }
        public override void OnSave()
        {
            Second = int.Parse(SecondBox.Text);
            Minute = int.Parse(MinuteBox.Text);
            Hour = int.Parse(HourBox.Text);
            Day = AlarmDatePick.SelectedDate.GetValueOrDefault().Day;
            Month = AlarmDatePick.SelectedDate.GetValueOrDefault().Month;
            Year = AlarmDatePick.SelectedDate.GetValueOrDefault().Year;
            /*Day = int.Parse(DayBox.Text);
            Month = int.Parse(MonthBox.Text);
            Year = int.Parse(YearBox.Text);
             */
            RecurDaily = (RecurringDailyCheckBox.IsChecked == true) ? true : false;
        }
        private string CheckValidity()
        {
            int seconds, minutes, hours, day, month, year;
            DateTime alarmTime;
            string s = "Invalid";

            bool badsec = !(int.TryParse(SecondBox.Text, out seconds) && (seconds >= 0 && seconds < 60));
            bool badmin = !(int.TryParse(MinuteBox.Text, out minutes) && (minutes >= 0 && minutes < 60));
            bool badhour = !(int.TryParse(HourBox.Text, out hours) && (hours >= 0 && hours < 24));
            bool badday = false;
            bool badmonth = false;
            bool badyear = false;
            bool badtotal = false;
            day = AlarmDatePick.SelectedDate.GetValueOrDefault().Day;
            month = AlarmDatePick.SelectedDate.GetValueOrDefault().Month;
            year = AlarmDatePick.SelectedDate.GetValueOrDefault().Year;
            if (RecurringDailyCheckBox.IsChecked == false)
            {
                //badday = !(int.TryParse(DayBox.Text, out day) && (day >= 1 && day < 32));
                //badmonth = !(int.TryParse(MonthBox.Text, out month) && (month >= 1 && month < 13));
                //badyear = !(int.TryParse(YearBox.Text, out year) && (year >= 2012 && year < 10000));
                if (!(badsec || badmin || badhour || badday || badmonth || badyear))
                {
                    string time = string.Format("{0}/{1}/{2} {3}:{4}:{5}", month, day, year, hours, minutes, seconds);
                    //MessageBox.Show(time +" " +DateTime.TryParseExact(time, "M/dd/yyyy h:m:s", CultureInfo.InvariantCulture, DateTimeStyles.None, out alarmTime).ToString());
                    badtotal = !(DateTime.TryParseExact(time, "M/d/yyyy H:m:s", CultureInfo.InvariantCulture, DateTimeStyles.None, out alarmTime) && (alarmTime.CompareTo(DateTime.Now)) > 0);
                    //MessageBox.Show(alarmTime.ToString());
                }
            }


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

            if (badday)
            {
                s += " days";
            }
            if (badmonth)
            {
                s += " month";
            }
            if (badyear)
            {
                s += " year";
            }

            if (badtotal && !(badsec || badmin || badhour || badday || badmonth || badyear))
                s = "Must be a valid Date and Time in the Future";

            CanSave = !(badsec || badmin || badhour || badday || badmonth || badyear || badtotal);
            return CanSave ? string.Empty : s;
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
