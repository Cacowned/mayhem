using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Threading;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using MayhemCore.ModuleTypes;

namespace DefaultModules.Events
{
    [DataContract]
    [MayhemModule("Timer", "Triggers after a certain amount of time")]
    public class Timer : EventBase, ICli, IWpfConfigurable
    {
        private DispatcherTimer myTimer;

        #region Configuration Properties
        [DataMember]
        private int Hours;

        [DataMember]
        private int Minutes;

        [DataMember]
        private int Seconds;
        #endregion

        protected override void Initialize()
        {
            myTimer = new DispatcherTimer();
            myTimer.Tick += new EventHandler(myTimer_Tick);
        }

        private void myTimer_Tick(object sender, EventArgs e)
        {
            Trigger();
        }

        public string GetConfigString()
        {
            return String.Format(CultureInfo.CurrentCulture, Strings.Timer_ConfigString, Hours, Minutes, Seconds);
        }

        #region Configuration Views
        public void CliConfig()
        {
            string TAG = "[Timer]";

            string input = "";
            int hours, minutes, seconds;

            do
            {
                Console.Write(Strings.Timer_CliConfig_HoursToWait, TAG);
                input = Console.ReadLine();
            }
            while (!Int32.TryParse(input, out hours) || !(hours >= 0));

            do
            {
                Console.Write(Strings.Timer_CliConfig_MinutesToWait, TAG);
                input = Console.ReadLine();
            }
            while (!Int32.TryParse(input, out minutes) || !(minutes >= 0 && minutes < 60));

            do
            {
                Console.Write(Strings.Timer_CliConfig_SecondsToWait, TAG);
                input = Console.ReadLine();
            }
            while (!Int32.TryParse(input, out seconds) || !(seconds >= 0 && seconds < 60));

            // everything checked out, set our variables
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new TimerConfig(Hours, Minutes, Seconds); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            TimerConfig config = (TimerConfig)configurationControl;
            Hours = config.Hours;
            Minutes = config.Minutes;
            Seconds = config.Seconds;
        }
        #endregion

        protected void SetInterval()
        {
            try
            {
                myTimer.Interval = new TimeSpan(Hours, Minutes, Seconds);
            }
            catch (Exception e)
            {
                Logger.WriteLine(Strings.Timer_CantSetInterval, e.Message);
            }
        }

        protected override bool OnEnable()
        {
            // Update our interval with the current values
            SetInterval();

            myTimer.Start();

            return true;
        }

        protected override void OnDisable()
        {
            myTimer.Stop();
        }
    }
}
