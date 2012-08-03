using System;
using System.Runtime.Serialization;
using System.Windows.Forms;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using System.IO;

namespace PowerModules
{
    [DataContract]
    [MayhemModule("2Power: Power Status", "This event triggers when a particular laptop power status is reached.")]
    public class PowerStatus2 : EventBase, IWpfConfigurable
    {
        [DataMember]
        private PowerStatusChoice chosenStatus;
        [DataMember]
        private float percentage;
        [DataMember]
        private BatteryChargeStatus chosenBCS;
        [DataMember]
        private int remainingTime; //minutes
        private Timer pollingTimer;
        private int pollingTimerInterval; //milliseconds
        private bool raiseEvent;
        #region config view
        public WpfConfiguration ConfigurationControl
        {
            get { return new PowerStatusConfiguration(chosenStatus, percentage, chosenBCS, remainingTime); }
        }
        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = (PowerStatusConfiguration)configurationControl;
            chosenStatus = config.ChosenStatus;
            chosenBCS = config.ChosenBCS;
            percentage = config.Percentage;
            remainingTime = config.RemainingTime;
            MessageBox.Show(chosenStatus.ToString() + " " + chosenBCS.ToString() + " " + percentage.ToString() + " " + remainingTime.ToString());
        }
        #endregion
        public string GetConfigString()
        {
            switch (chosenStatus)
            {
                case PowerStatusChoice.Percentage:
                    return string.Format("Battery falls to {0}%", percentage);
                case PowerStatusChoice.RemainingTime:
                    return string.Format("{0} minutes battery time remaining", remainingTime.ToString());
                default:
                    return "Battery hits " + chosenBCS.ToString() + " status";
            }
        }
        protected override void OnLoadDefaults()
        {
            chosenStatus = PowerStatusChoice.PowerState;
            chosenBCS = BatteryChargeStatus.Charging;
            remainingTime = 30*60;
            percentage = 30;
        }
        protected override void OnAfterLoad()
        {
            TestAndSetRaiseEvent();
            pollingTimerInterval = 3000; //3 seconds
            pollingTimer = new Timer();
            pollingTimer.Enabled = true;
            pollingTimer.Interval = pollingTimerInterval;
            pollingTimer.Tick += new EventHandler(ConditionCheck);
        }

        private void TestAndSetRaiseEvent()
        {
            raiseEvent = false;
            switch (chosenStatus)
            {
                case PowerStatusChoice.PowerState:
                    switch (chosenBCS)
                    {
                        case BatteryChargeStatus.Charging:
                        case BatteryChargeStatus.Critical:
                            if ((chosenBCS & SystemInformation.PowerStatus.BatteryChargeStatus) != chosenBCS)
                            {
                                raiseEvent = true;
                            }
                            break;
                        case BatteryChargeStatus.Low: 
                            if ((BatteryChargeStatus.High & SystemInformation.PowerStatus.BatteryChargeStatus) != BatteryChargeStatus.High)
                            {
                                raiseEvent = true;
                            }
                            break;
                    }
                    break;
                case PowerStatusChoice.Percentage:
                    if (BatteryPercentageRemaining() > percentage)
                    {
                        MessageBox.Show("raiseevent true " + percentage.ToString() + " < " + BatteryPercentageRemaining());
                        raiseEvent = true;
                    }
                    break;
                case PowerStatusChoice.RemainingTime:
                    if (BatteryMinutesRemaining() > remainingTime)
                        raiseEvent = true;
                    break;
                default:
                    MessageBox.Show("cond check fail");
                    break;
            }
        }
        protected int BatteryMinutesRemaining()
        {
            return (int)(SystemInformation.PowerStatus.BatteryLifeRemaining / 60);
        }
        protected int BatteryPercentageRemaining()
        {
            return (int)(SystemInformation.PowerStatus.BatteryLifePercent*100);
        }
        protected override void OnDisabled(DisabledEventArgs e)
        {
            MessageBox.Show("disabled rasie event false");
            raiseEvent = false;
            pollingTimer.Stop();
            //pollingTimer.Dispose();
        }
        protected override void OnEnabling(EnablingEventArgs e)
        {
            /*pollingTimer = new Timer();
            pollingTimer.Enabled = true;
            pollingTimer.Interval = pollingTimerInterval;
            pollingTimer.Tick += new EventHandler(ConditionCheck);
             */
            pollingTimer.Start();
            TestAndSetRaiseEvent();
        }
        private void callTrigger()
        {
            MessageBox.Show("call trigger raise event false");
            raiseEvent = false;
            Trigger();
        }
        private void ConditionCheck(Object sender, EventArgs e)
        {
            MessageBox.Show("in cond check");
            if (raiseEvent)
            {
                switch (chosenStatus)
                {
                    case PowerStatusChoice.PowerState:
                        if ((chosenBCS & SystemInformation.PowerStatus.BatteryChargeStatus) == chosenBCS)
                        {
                            MessageBox.Show(SystemInformation.PowerStatus.BatteryChargeStatus.ToString());
                            callTrigger();
                        }
                        break;
                    case PowerStatusChoice.Percentage:
                        MessageBox.Show("in the right place "+percentage.ToString()+" "+BatteryPercentageRemaining());
                        if (percentage <= BatteryPercentageRemaining())
                        {
                            //MessageBox.Show(percentage.ToString() + " " + BatteryPercentageRemaining().ToString());
                            callTrigger();
                        }
                        break;
                    case PowerStatusChoice.RemainingTime:
                        if (remainingTime <= BatteryMinutesRemaining())
                            callTrigger();
                        break;
                }
            }
            else
            {
                TestAndSetRaiseEvent();
            }
        }
    }
}
