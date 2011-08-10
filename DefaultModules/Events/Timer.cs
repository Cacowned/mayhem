using System;
using System.Runtime.Serialization;
using System.Timers;
using System.Windows;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Windows.Controls;
using MayhemDefaultStyles.UserControls;

namespace DefaultModules.Events
{
    [MayhemModule("Timer", "Triggers after a certain amount of time")]
    public class Timer : EventBase, ICli, IWpfConfigurable
    {
        private System.Timers.Timer myTimer;

        #region Configuration Properties
        private int _hours;
        [DataMember]
        private int Hours
        {
            get
            {
                return _hours;
            }
            set
            {
                _hours = value;
                SetConfigString();
            }
        }

        private int _minutes;
        [DataMember]
        private int Minutes
        {
            get
            {
                return _minutes;
            }
            set
            {
                _minutes = value;
                SetConfigString();
            }
        }

        private int _seconds;
        [DataMember]
        private int Seconds
        {
            get
            {
                return _seconds;
            }
            set
            {
                _seconds = value;
                SetConfigString();
            }
        }
        #endregion


        public Timer()
        { }

        protected override void Initialize()
        {
            base.Initialize();

            myTimer = new System.Timers.Timer();
            myTimer.Elapsed += new ElapsedEventHandler(myTimer_Elapsed);
            myTimer.Enabled = false;

            // Set our defaults
            Seconds = 2;
        }

        public override void SetConfigString()
        {
            ConfigString = String.Format("{0} hours, {1} minutes, {2} seconds", Hours, Minutes, Seconds);
        }

        #region Configuration Views
        public void CliConfig()
        {
            string TAG = "[TIMER]";

            string input = "";
            int hours, minutes, seconds;

            do {
                Console.Write("{0} Please enter the number of hours to wait: ", TAG);
                input = Console.ReadLine();
            }
            while (!Int32.TryParse(input, out hours) || !(hours >= 0));

            do {
                Console.Write("{0} Please enter the number of minutes to wait: ", TAG);
                input = Console.ReadLine();
            }
            while (!Int32.TryParse(input, out minutes) || !(minutes >= 0 && minutes < 60));

            do {
                Console.Write("{0} Please enter the number of seconds to wait: ", TAG);
                input = Console.ReadLine();
            }
            while (!Int32.TryParse(input, out seconds) || !(seconds >= 0 && seconds < 60));

            // everything checked out, set our variables
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
        }

        /*
        public void WpfConfig()
        {
            var window = new TimerConfig(Hours, Minutes, Seconds);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (window.ShowDialog() == true) {

                Hours = window.hours;
                Minutes = window.minutes;
                Seconds = window.seconds;
            }
        }
         */

        public IWpfConfiguration ConfigurationControl
        {
            get { return new TimerConfig(Hours, Minutes, Seconds); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            Hours = ((TimerConfig)configurationControl).Hours;
            Minutes = ((TimerConfig)configurationControl).Minutes;
            Seconds = ((TimerConfig)configurationControl).Seconds;
        }
        #endregion


        private void myTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            base.OnEventActivated();
        }

        protected void SetInterval()
        {
            double interval = (Hours * 3600 + Minutes * 60 + Seconds) * 1000;

            try {
                myTimer.Interval = interval;
            }
            catch {
                /* setting the interval throws if the 
                 * given argument is less than or equal to 0
                 */
            }
        }

        public override void Enable()
        {
            // Update our interval with the current values
            SetInterval();

            base.Enable();
            myTimer.Enabled = true;
            myTimer.Start();
        }

        public override void Disable()
        {
            base.Disable();
            myTimer.Stop();
            myTimer.Enabled = false;
        }
    }
}
