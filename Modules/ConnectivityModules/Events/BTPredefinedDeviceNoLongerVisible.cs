using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Timers;
using ConnectivityModule.Wpf;
using InTheHand.Net;
using InTheHand.Net.Sockets;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Events
{
    [DataContract]
    [MayhemModule("Bluetooth: Predefined Device No Longer Visible", "Triggers when the selected device is no longer visible")]
    public class BTPredefinedDeviceNoLongerVisible : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string deviceName;

        [DataMember]
        private int seconds;

        private BluetoothAddress deviceAddress;
        private BluetoothClient bluetoothClient;
        private Timer timer;

        private List<BluetoothDeviceInfo> devices;

        private bool isVisible;
        private bool isFirstRun;

        protected override void OnEnabling(EnablingEventArgs e)
        {
            timer = new Timer();
            timer.Interval = 100; // Setting the timer to 0.1 seconds so that it will get the initial list right away
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            isVisible = true;
            isFirstRun = true;
            deviceAddress = null;

            try
            {
                bluetoothClient = new BluetoothClient();
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
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (this)
            {
                timer.Stop();

                try
                {
                    if (isFirstRun)
                    {
                        timer.Interval = seconds * 1000;
                        isFirstRun = false;
                    }

                    List<BluetoothDeviceInfo> newDevices = bluetoothClient.DiscoverDevices(10, false, false, false, true).ToList();

                    bool found = false;

                    // We get the current available devices and we see if the device we are looking for isn't in the list. If so we trigger the event
                    foreach (BluetoothDeviceInfo device in newDevices)
                    {
                        // It is possible that the name of the device is not received correctly, so when we manage to get it's address for one time we will save it and use it also for compare 
                        if ((deviceAddress != null && device.DeviceAddress.Equals(deviceAddress)) || device.DeviceName.Equals(deviceName))
                        {
                            found = true; // The device we search for is in the list of the available devices
                            deviceAddress = device.DeviceAddress;

                            if (isVisible == false)
                            {
                                isVisible = true;

                                break;
                            }
                        }
                    }

                    // If the device was previously found but is no longer in reach we trigger the event
                    if (isVisible == true && found == false)
                    {
                        isVisible = false;
                        Trigger();
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

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new DeviceNameConfig(deviceName, seconds); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as DeviceNameConfig;

            if (config == null)
                return;

            deviceName = config.DeviceName;
            seconds = config.Seconds;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.DeviceName_ConfigString, deviceName);
        }

        #endregion
    }
}
