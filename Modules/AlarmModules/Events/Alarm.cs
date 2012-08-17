using System;
using System.Runtime.Serialization;
using AlarmModules.Resources;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using Schedule;

namespace AlarmModules
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

        private ScheduleTimer tickTimer;

        private DateTime alarmTime;

        #region Config View
        public WpfConfiguration ConfigurationControl
        {
            get { return new AlarmConfig(recurDaily, day, month, year, hour, minute, second); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = (AlarmConfig)configurationControl;
            hour = config.Hour;
            minute = config.Minute;
            second = config.Second;
            day = config.Day;
            month = config.Month;
            year = config.Year;
            recurDaily = config.RecurDaily;
        }

    #endregion
        
        protected override void OnEnabling(EnablingEventArgs e)
        {
            tickTimer = new ScheduleTimer();
            tickTimer.Elapsed += AlarmHit;

            if (recurDaily)
            {
                tickTimer.AddEvent(new Schedule.ScheduledTime(EventTimeBase.Daily, new TimeSpan(0, hour, minute, second, 0)));
            }
            else
            {
                alarmTime = new DateTime(year, month, day, hour, minute, second);
                SingleEvent se = new SingleEvent(alarmTime);
                tickTimer.AddEvent(se);
            }

            tickTimer.Start(); 
        }
        
        private void AlarmHit(object sender, EventArgs e)
        {
            Trigger();
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            tickTimer.Stop();
            tickTimer.Dispose();
        }

        public string GetConfigString()
        {
            string retString;

            if (recurDaily)
            {
                retString = string.Format(Strings.DailyAlarmConfig, hour, minute, second);
            }
            else
            {
                retString = string.Format(Strings.OneTimeAlarmConfig, month, day, year, hour, minute, second);
            }

            return retString;
        }
    }
}

