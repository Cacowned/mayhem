using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using Schedule;


namespace PreGsocTest1
{
    [DataContract]
    [MayhemModule("Alarm","This event triggers at a particular time of the day.")]
    public class Alarm : EventBase, IWpfConfigurable
    {
        [DataMember]
        private int hour;
        [DataMember]
        private int minute;
        [DataMember]
        private int second;
        [DataMember]
        private int day;
        [DataMember]
        private int month;
        [DataMember]
        private int year;
        [DataMember]
        private bool recurDaily;
        private ScheduleTimer TickTimer;
        private DateTime alarmTime;

        #region config view
        public WpfConfiguration ConfigurationControl
        {
            get { return new DPAlarmConfig(recurDaily, day, month, year, hour, minute, second); }
        }
        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = (DPAlarmConfig)configurationControl;
            hour = config.Hour;
            minute = config.Minute;
            second = config.Second;
            day = config.Day;
            month = config.Month;
            year = config.Year;
            recurDaily = config.RecurDaily;
        }
    #endregion
        protected override void OnLoadDefaults()
        {
        }
        protected override void OnLoadFromSaved()
        {
        }
        protected override void OnAfterLoad()
        {
              
        }
        protected override void OnEnabling(EnablingEventArgs e)
        {
            TickTimer = new ScheduleTimer();
            TickTimer.Elapsed += new ScheduledEventHandler(AlarmHit); 
            if (recurDaily)
            {
                //TickTimer.AddEvent(new Schedule.ScheduledTime("Daily", "7:07  AM"));
                TickTimer.AddEvent(new Schedule.ScheduledTime(EventTimeBase.Daily, new TimeSpan(0, hour, minute, second, 0)));
            }
            else
            {
                alarmTime = new DateTime(year, month, day, hour, minute, second);
                SingleEvent se = new SingleEvent(alarmTime);
                TickTimer.AddEvent(se);
            }
            TickTimer.Start(); 
        }
        private void AlarmHit(object sender, EventArgs e)
        {
            Trigger();
        }
        protected override void OnDisabled(DisabledEventArgs e)
        {
            TickTimer.Stop();
        }
        public string GetConfigString()
        {
            string retString;
            if (recurDaily)
                retString = "Daily, " + new DateTime(year,month,day,hour,minute,second).ToLongTimeString();
            else
            {
                retString = string.Format("One Time, {0}/{1}/{2} ",month,day,year);
                retString = "One Time, " + new DateTime(year, month, day, hour, minute, second).ToString();
            }
            return retString;
        }
    }
}
