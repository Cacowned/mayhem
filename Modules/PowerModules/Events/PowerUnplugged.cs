using System;
using System.Windows.Forms;
using MayhemCore;

namespace PowerModules
{
    [MayhemModule("Power: Cable Unplugged", "This event triggers when the power cable is unplugged.")]
    public class PowerUnplugged : EventBase
    {
        private bool _plugged;
        private bool raiseEvent; //makes sure to raise the event only once per unplug
        private Timer plugStateCheck;

        public bool Plugged
        {
            get { return _plugged; }
            set
            {
                _plugged = value;
                if (!_plugged && raiseEvent)
                {
                        raiseEvent = false;
                        Trigger();
                        plugStateCheck.Stop();
                }        
                else if(_plugged && !raiseEvent)
                {
                        raiseEvent = true;
                        plugStateCheck.Start();
                }
            }
        }

        private void CableUnpluggedCheck(object sender, EventArgs e)
        {
            Plugged = (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online);
        }

        protected override void OnAfterLoad()
        {
            plugStateCheck = new Timer();
            plugStateCheck.Enabled = true;
            plugStateCheck.Interval = 2000; //2 seconds
            plugStateCheck.Tick += CableUnpluggedCheck;
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            raiseEvent = false;
            plugStateCheck.Stop();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            plugStateCheck.Start();
            raiseEvent = true;
        }
    }
}
