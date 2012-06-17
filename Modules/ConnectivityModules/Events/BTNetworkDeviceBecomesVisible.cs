using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InTheHand.Net.Sockets;
using MayhemCore;
using InTheHand.Net.Bluetooth;

namespace ConnectivityModule.Events
{
    public class BTNetworkDeviceBecomesVisible : EventBase
    {
        private BluetoothListener bl;

        protected override void OnEnabling(EnablingEventArgs e)
        {
            BluetoothRadio br = BluetoothRadio.PrimaryRadio;

            if (br == null)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.BT_NoBluetooth);
                e.Cancel = true;
            }

            BluetoothClient bc = new BluetoothClient();
             bc.DiscoverDevices();
        }
    }
}
