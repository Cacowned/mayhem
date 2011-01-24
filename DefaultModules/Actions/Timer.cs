using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Timers;

namespace DefaultModules.Actions
{
    public class Timer : ActionBase, ICli
    {
        protected const string TAG = "[Timer]";

        protected int hours, minutes, seconds = 0;

        private System.Timers.Timer myTimer;
        //public override event ActionActivateHandler OnActionActivated;

        public Timer()
            : base("Timer", "Triggers after a certain amount of time")
        {
            hasConfig = true;

            hours = 0;
            minutes = 0;
            seconds = 3;

            myTimer = new System.Timers.Timer();
            myTimer.Elapsed += new ElapsedEventHandler(myTimer_Elapsed);
            myTimer.Enabled = false;
            
        }

        public void CliConfig() {
            
            string input = "";
            
            do{
                Console.Write("{0} Please enter the number of hours to wait: ", TAG);
                input =  Console.ReadLine();
            }
            while(Int32.TryParse(input, out hours) && !(hours >= 0));

            do {
                Console.Write("{0} Please enter the number of minutes to wait: ", TAG);
                input = Console.ReadLine();
            }
            while (Int32.TryParse(input, out minutes) && !(minutes >= 0 && minutes < 60));

            do {
                Console.Write("{0} Please enter the number of seconds to wait: ", TAG);
                input = Console.ReadLine();
            }
            while (Int32.TryParse(input, out seconds) && !(seconds >= 0 && seconds < 60));

            double interval = (hours * 3600 + minutes * 60 + seconds) * 1000;
            myTimer.Interval = interval;

        }

        private void myTimer_Elapsed(object sender, ElapsedEventArgs e) {
            base.OnActionActivated();
        }

        public override void Enable() {
            myTimer.Enabled = true;
            myTimer.Start();
        }

        public override void Disable() {
            myTimer.Stop();
            myTimer.Enabled = false;
        }
    }
}
