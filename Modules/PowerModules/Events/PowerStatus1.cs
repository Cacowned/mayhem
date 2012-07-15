﻿using System;
using System.Runtime.Serialization;
using System.Windows.Forms;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace PreGsocTest1
{
    [DataContract]
    [MayhemModule("1Power: Power Status", "This event triggers when a particular laptop power status is reached.")]
    public class Paar : EventBase, IWpfConfigurable
    {
        [DataMember]
        private PowerStatusChoice chosenStatus;
        [DataMember]
        private float percentage;
        [DataMember]
        private BatteryChargeStatus chosenBCS;
        [DataMember]
        private int remainingTime;
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
            remainingTime = 30;
            percentage = 30;
        }
        protected override void OnAfterLoad()
        {
            pollingTimerInterval = 2000; //2 seconds
            pollingTimer = new Timer();
            pollingTimer.Enabled = true;
            pollingTimer.Interval = pollingTimerInterval;
            pollingTimer.Tick += new EventHandler(ConditionCheck);
        }
        protected override void OnDisabled(DisabledEventArgs e)
        {
            raiseEvent = false;
            pollingTimer.Stop();
            pollingTimer.Dispose();
        }
        protected override void OnEnabling(EnablingEventArgs e)
        {
            pollingTimer.Start();
            raiseEvent = true;
        }
        private void callTrigger()
        {
            raiseEvent = false;
            Trigger();
        }
        private void ConditionCheck(Object sender, EventArgs e)
        {
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
                        //MessageBox.Show("in the right place");
                        float diff = Math.Abs(percentage - (SystemInformation.PowerStatus.BatteryLifePercent * 100));
                        if (diff < 1)
                        {
                            //MessageBox.Show(diff.ToString());
                            callTrigger();
                        }
                        break;
                    case PowerStatusChoice.RemainingTime:
                        if (remainingTime <= SystemInformation.PowerStatus.BatteryLifeRemaining)
                            callTrigger();
                        break;
                }
            }
            else
            {
            }
        }
        private void ConditionCheck1(Object sender, EventArgs e)
        {
            if (!raiseEvent)
            {
                if (chosenStatus == PowerStatusChoice.PowerState && ((chosenBCS & BatteryChargeStatus.Charging)==chosenBCS))
                {
                    if ((SystemInformation.PowerStatus.BatteryChargeStatus & BatteryChargeStatus.Charging)!=BatteryChargeStatus.Charging)
                        raiseEvent = true;
                }
                else
                {
                    if ((SystemInformation.PowerStatus.BatteryChargeStatus & BatteryChargeStatus.Charging)==BatteryChargeStatus.Charging)
                        raiseEvent = true;
                }
                return;
            }
            else
            {
                //MessageBox.Show(SystemInformation.PowerStatus.BatteryChargeStatus.ToString());
                //MessageBox.Show(chosenBCS.ToString());
                //MessageBox.Show(chosenStatus.ToString());
                if (!(chosenStatus == PowerStatusChoice.PowerState && ((chosenBCS & BatteryChargeStatus.Charging)==chosenBCS))) //ensures one trigger everytime plug is disconnected. we dont want evetns while charging.
                {
                    if ((SystemInformation.PowerStatus.BatteryChargeStatus & BatteryChargeStatus.Charging)==BatteryChargeStatus.Charging)
                        return;
                }
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
                        if (percentage <= (SystemInformation.PowerStatus.BatteryLifePercent * 100))
                            callTrigger();
                        break;
                    case PowerStatusChoice.RemainingTime:
                        if (remainingTime <= SystemInformation.PowerStatus.BatteryLifeRemaining)
                            callTrigger();
                        break;
                }
            }
        }
    }
}
