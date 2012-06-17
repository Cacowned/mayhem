using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Timers;
using InTheHand.Net.Sockets;
using MayhemCore;

namespace ConnectivityModule.Events
{
    [DataContract]
    [MayhemModule("Bluetooth: New Device Visible", "Triggers when a device becomes visible")]
    public class BTDeviceBecomesVisible : EventBase
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

            timer = new Timer();
            timer.Interval = 100; // Setting the timer to 0.1 seconds so that it will get the initial list right away
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
                        // If it's the first time the timer is set we get the list
                        devices = new List<BluetoothDeviceInfo>();
                        devices.AddRange(newDevices);
                        timer.Interval = int.Parse(Strings.General_TimerInterval);
                    }
                    else
                    {
                        auxDevices.Clear();
                        // We get the current available devices and we see if there is a new device in the list. If so we trigger the event and add the new device to the list
                        foreach (BluetoothDeviceInfo device in newDevices)
                            if (!devices.Contains(device))
                                auxDevices.Add(device);

                        foreach (BluetoothDeviceInfo device in auxDevices)
                        {
                            devices.Add(device);
                            Trigger();
                        }


                        auxDevices.Clear();
                        // We make a search to see if the device is no longer visible, if so, we need to remove it from the list
                        foreach (BluetoothDeviceInfo device in devices)
                            if (!newDevices.Contains(device))
                                auxDevices.Add(device);

                        foreach (BluetoothDeviceInfo device in auxDevices)
                            devices.Remove(device);
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
