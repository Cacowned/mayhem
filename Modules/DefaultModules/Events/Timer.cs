using System;
using System.Runtime.Serialization;
using System.Windows.Threading;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;

namespace DefaultModules.Events
{
    [DataContract]
    [MayhemModule("Timer", "Triggers after a certain amount of time")]
    public class Timer : EventBase, ICli, IWpfConfigurable
    {
        private DispatcherTimer myTimer;

        #region Configuration Properties
        [DataMember]
        private int Hours
        {
            get;
            set;
        }

        [DataMember]
        private int Minutes
        {
            get;
            set;
        }

        [DataMember]
        private int Seconds
        {
            get;
            set;
        }
        #endregion
        
        protected override void Initialize()
        {
            base.Initialize();

            myTimer = new DispatcherTimer();
            myTimer.Tick += new EventHandler(myTimer_Tick);
        }

        void myTimer_Tick(object sender, EventArgs e)
        {
            OnEventActivated();
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

            do
            {
                Console.Write("{0} Please enter the number of hours to wait: ", TAG);
                input = Console.ReadLine();
            }
            while (!Int32.TryParse(input, out hours) || !(hours >= 0));

            do
            {
                Console.Write("{0} Please enter the number of minutes to wait: ", TAG);
                input = Console.ReadLine();
            }
            while (!Int32.TryParse(input, out minutes) || !(minutes >= 0 && minutes < 60));

            do
            {
                Console.Write("{0} Please enter the number of seconds to wait: ", TAG);
                input = Console.ReadLine();
            }
            while (!Int32.TryParse(input, out seconds) || !(seconds >= 0 && seconds < 60));

            // everything checked out, set our variables
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
        }

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

        protected void SetInterval()
        {

            try
            {
                myTimer.Interval = new TimeSpan(Hours, Minutes, Seconds);
            }
            catch(Exception e)
            {
                Logger.WriteLine("Can't set timer interval. Exception: {0}", e.Message);
            }
        }

        public override void Enable()
        {
            // Update our interval with the current values
            SetInterval();

            base.Enable();
            myTimer.Start();
        }

        public override void Disable()
        {
            base.Disable();
            myTimer.Stop();
        }
    }
}
