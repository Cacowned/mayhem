using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using InTheHand.Net.Sockets;
using MayhemCore;

namespace ConnectivityModule.Events
{
    [MayhemModule("Bluetooth: Device No Longer Visible", "Triggers when a device is no longer visible")]
    public class BTDeviceNoLongerVisible : EventBase
    {
        private BluetoothClient bluetoothClient;
        private Timer timer;

        private List<BluetoothDeviceInfo> devices;
        private List<BluetoothDeviceInfo> auxDevices;
        private List<BluetoothDeviceInfo> newDevices;

        protected override void OnEnabling(EnablingEventArgs e)
        {
            devices = null;
            auxDevices = null;
            newDevices = null;

            timer = new System.Timers.Timer();
            timer.Interval = 100;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            try
            {
                bluetoothClient = new BluetoothClient();
                auxDevices = new List<BluetoothDeviceInfo>();

                timer.Start();
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.BT_NoBluetooth);
                Logger.Write(ex);
                e.Cancel = true;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }

            if (devices != null)
            {
                devices.Clear();
                devices = null;
            }

            if (auxDevices != null)
            {
                auxDevices.Clear();
                auxDevices = null;
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (this)
            {
                timer.Stop();

                try
                {
                    newDevices = bluetoothClient.DiscoverDevices(10, false, false, false, true).ToList();

                    if (devices == null)
                    {
                        devices = new List<BluetoothDeviceInfo>();
                        devices.AddRange(newDevices);
                        timer.Interval = int.Parse(Strings.General_TimerInterval);
                    }
                    else
                    {
                        auxDevices.Clear();

                        foreach (BluetoothDeviceInfo device in newDevices)
                            if (!devices.Contains(device))
                                auxDevices.Add(device);

                        foreach (BluetoothDeviceInfo device in auxDevices)
                            devices.Add(device);

                        auxDevices.Clear();

                        foreach (BluetoothDeviceInfo device in devices)
                            if (!newDevices.Contains(device))
                                auxDevices.Add(device);

                        foreach (BluetoothDeviceInfo device in auxDevices)
                        {
                            devices.Remove(device);
                            Trigger();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.BT_ErrorMonitoringBluetooth);
                    Logger.Write(ex);
                }

                timer.Start();
            }
        }
    }
}
