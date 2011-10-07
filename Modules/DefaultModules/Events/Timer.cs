using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Threading;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DefaultModules.Events
{
    [DataContract]
    [MayhemModule("Timer", "Triggers after a certain amount of time")]
    public class Timer : EventBase, IWpfConfigurable
    {
        [DataMember]
        private int hours;

        [DataMember]
        private int minutes;

        [DataMember]
        private int seconds;

        private DispatcherTimer myTimer;

        protected override void OnAfterLoad()
        {
            myTimer = new DispatcherTimer();
            myTimer.Tick += myTimer_Tick;
        }

        private void myTimer_Tick(object sender, EventArgs e)
        {
            Trigger();
        }

        public string GetConfigString()
        {
            return String.Format(CultureInfo.CurrentCulture, Strings.Timer_ConfigString, hours, minutes, seconds);
        }

        #region Configuration Views

        public WpfConfiguration ConfigurationControl
        {
            get { return new TimerConfig(hours, minutes, seconds); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            TimerConfig config = (TimerConfig)configurationControl;
            hours = config.Hours;
            minutes = config.Minutes;
            seconds = config.Seconds;
        }
        #endregion

        protected void SetInterval()
        {
            try
            {
                myTimer.Interval = new TimeSpan(hours, minutes, seconds);
            }
            catch (Exception e)
            {
                Logger.WriteLine(Strings.Timer_CantSetInterval, e.Message);
            }
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            // Update our interval with the current values
            SetInterval();

            myTimer.Start();
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            myTimer.Stop();
        }
    }
}
