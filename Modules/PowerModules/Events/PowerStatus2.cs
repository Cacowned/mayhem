using System;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Threading;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace PowerModules
{
    public enum PowerStatusChoice
    {
        Percentage, PowerState
    }
    [DataContract]
    [MayhemModule("Power: Power Status", "This event triggers when a particular laptop power status is reached.")]
    public class PowerStatus : EventBase, IWpfConfigurable
    {
        [DataMember]
        private PowerStatusChoice chosenStatus;
        [DataMember]
        private int percentage;
        [DataMember]
        private BatteryChargeStatus chosenBCS;
        private DispatcherTimer pollingTimer;
        private bool raiseEvent;
        #region config view
        public WpfConfiguration ConfigurationControl
        {
            get { return new PowerStatusConfiguration(chosenStatus, chosenBCS, percentage); }
        }
        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = (PowerStatusConfiguration)configurationControl;
            chosenStatus = config.ChosenStatus;
            chosenBCS = config.ChosenBCS;
            percentage = config.Percentage;
            //MessageBox.Show(chosenStatus.ToString() + " " + chosenBCS.ToString() + " " + percentage.ToString() + " " + remainingTime.ToString());
        }
        #endregion
        public string GetConfigString()
        {
            switch (chosenStatus)
            {
                case PowerStatusChoice.Percentage:
                    return string.Format("Battery falls to {0}%", percentage);
                default:
                    return "Battery hits " + chosenBCS.ToString() + " status";
            }
        }
        protected override void OnLoadDefaults()
        {
            chosenStatus = PowerStatusChoice.PowerState;
            chosenBCS = BatteryChargeStatus.Charging;
            percentage = 30;
        }
        protected override void OnAfterLoad()
        {
            pollingTimer = new DispatcherTimer();
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
                    if (BatteryPercentageRemaining() > (int)percentage)
                    {
                        //MessageBox.Show("raiseevent true " + percentage.ToString() + " < " + BatteryPercentageRemaining());
                        raiseEvent = true;
                    }
                    break;
                default:
                    //MessageBox.Show("cond check fail");
                    break;
            }
        }
        protected int BatteryPercentageRemaining()
        {
            return (int)(SystemInformation.PowerStatus.BatteryLifePercent*100);
        }
        protected override void OnDisabled(DisabledEventArgs e)
        {
            //MessageBox.Show("disabled rasie event false");
            raiseEvent = false;
            pollingTimer.Stop();
        }
        protected override void OnEnabling(EnablingEventArgs e)
        {
            TestAndSetRaiseEvent();
            pollingTimer.Interval = TimeSpan.FromSeconds(3);
            pollingTimer.Tick += new EventHandler(ConditionCheck);
            pollingTimer.Start();
        }
        private void callTrigger()
        {
            //MessageBox.Show("call trigger raise event false");
            raiseEvent = false;
            Trigger();
        }
        private void ConditionCheck(Object sender, EventArgs e)
        {
            //MessageBox.Show("in cond check");
            if (raiseEvent)
            {
                switch (chosenStatus)
                {
                    case PowerStatusChoice.PowerState:
                        if ((chosenBCS & SystemInformation.PowerStatus.BatteryChargeStatus) == chosenBCS)
                        {
                            //MessageBox.Show(SystemInformation.PowerStatus.BatteryChargeStatus.ToString());
                            callTrigger();
                        }
                        break;
                    case PowerStatusChoice.Percentage:
                        //MessageBox.Show("in the right place "+percentage.ToString()+" "+BatteryPercentageRemaining());
                        if ((int)percentage >= BatteryPercentageRemaining())
                        {
                            //MessageBox.Show(percentage.ToString() + " " + BatteryPercentageRemaining().ToString());
                            callTrigger();
                        }
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
