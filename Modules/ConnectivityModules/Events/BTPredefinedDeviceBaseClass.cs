using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Timers;
using InTheHand.Net;
using InTheHand.Net.Sockets;
using MayhemCore;

namespace ConnectivityModule.Events
{
    /// <summary>
    /// An abstract base class used for detecting when a predefined bluetooth devices becomes or is no longer visible.
    /// </summary>
    [DataContract]
    public abstract class BTPredefinedDeviceBaseClass : EventBase
    {
        /// <summary>
        /// The name of the device that is monitored.
        /// </summary>
        [DataMember]
        protected string deviceName;

        /// <summary>
        /// The time in seconds that is waited before another check is made. The minimum timespan is 5 seconds.
        /// </summary>
        [DataMember]
        protected int seconds;

        protected BluetoothAddress deviceAddress;
        protected BluetoothClient bluetoothClient;
        protected Timer timer;

        protected List<BluetoothDeviceInfo> devices;

        /// <summary>
        /// This field will tell if we monitor a device to be no longer be visible or to become visible.
        /// </summary>
        protected string monitorType;

        protected bool isVisible;
        protected bool wasVisible;
        protected bool isFirstRun;

        /// <summary>
        /// This method is initializing the needed objects and starts the timer.
        /// If the bluetooth capability is not available for this computer the event will not be enabled and an error message will be shown.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            timer = new Timer();
            timer.Interval = 100; // Setting the timer to 0.1 seconds so that it will get the initial list right away
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            isVisible = false;
            wasVisible = false;
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

        /// <summary>
        /// This method is releasing the used objects when the event is disabled.
        /// </summary>
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

        /// <summary>
        /// This method checks about changes in a device's visibility and it's updating the isVisible and wasVisible fields when changes occur.
        /// </summary>
        /// <returns>Returns true if the operations of this method completed successfully, false otherwise </returns>
        protected bool VerifyDeviceVisibility()
        {
            if (isFirstRun)
            {
                timer.Interval = seconds * 1000;
                isFirstRun = false;
            }

            try
            {
                // Calling this method will return a list of the currently visible devices.
                // Note that this method takes up to 10 seconds to complete, in order to have accurate results.
                List<BluetoothDeviceInfo> newDevices = bluetoothClient.DiscoverDevices(10, false, false, false, true).ToList();

                bool found = false;

                // We get the current available devices and we see if the device we are looking for is in the list. If so we update the values of isVisible and wasVisible.
                foreach (BluetoothDeviceInfo device in newDevices)
                {
                    // It is possible that the name of the device is not received correctly, so when we manage to get it's address we will save it and also use it for comparison.
                    if ((deviceAddress != null && device.DeviceAddress.Equals(deviceAddress)) || device.DeviceName.Equals(deviceName))
                    {
                        if (isFirstRun)
                        {
                            isVisible = true;
                            wasVisible = true;
                        }

                        // The device we search for is in the list of the available devices.
                        found = true;
                        deviceAddress = device.DeviceAddress;

                        if (!isVisible)
                        {
                            wasVisible = false;
                            isVisible = true;
                        }

                        break;
                    }
                }

                if (!found && isVisible)
                {
                    wasVisible = true;
                    isVisible = false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.BT_ErrorMonitoringBluetooth);
                Logger.Write(ex);

                return false;
            }
        }

        /// <summary>
        /// This method will be implemented by the classes that inherit this class and will be called when the timer.Elapsed event will be raised.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected abstract void timer_Elapsed(object sender, ElapsedEventArgs e);
    }
}
