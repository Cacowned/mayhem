using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Timers;
using InTheHand.Net.Sockets;
using MayhemCore;

namespace ConnectivityModule.Events
{
    /// <summary>
    /// An abstract base class used for detecting when a bluetooth device has become or is no longer visible.
    /// </summary>
    public abstract class BTDeviceBaseClass : EventBase
    {
        protected BluetoothClient bluetoothClient;
        protected Timer timer;

        protected List<BluetoothDeviceInfo> devices;
        protected List<BluetoothDeviceInfo> newDevices;
        protected List<BluetoothDeviceInfo> auxDevices;

        /// <summary>
        /// This method is initializing the needed objects and starts the timer.
        /// If the bluetooth capability is not available for this computer the event will not be enabled and an error message will be shown.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            devices = null;
            auxDevices = null;
            newDevices = null;

            timer = new Timer();
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

            if (auxDevices != null)
            {
                auxDevices.Clear();
                auxDevices = null;
            }
        }

        /// <summary>
        /// This method is setting the list of visible devices for the first time.
        /// The normal interval for the timer is set.
        /// </summary>
        protected void MakeFirstListOfDevices()
        {
            devices = new List<BluetoothDeviceInfo>();
            devices.AddRange(newDevices);

            timer.Interval = int.Parse(Strings.General_BluetoothTimerInterval);
        }

        /// <summary>
        /// This method is searching the newDevices list and finds the devices that has become available since the last check. The found devices are added to the main list.
        /// </summary>
        /// <returns>Returns true if a new device is found, false otherwise</returns>
        protected bool FindNewDevices()
        {
            bool found = false;

            auxDevices.Clear();

            // We get the current available devices and we see if there is a new device in the list. If so we add the new device to the main list.
            foreach (BluetoothDeviceInfo device in newDevices)
            {
                if (!devices.Contains(device))
                {
                    auxDevices.Add(device);
                }
            }

            foreach (BluetoothDeviceInfo device in auxDevices)
            {
                devices.Add(device);
                found = true;
            }

            return found;
        }

        /// <summary>
        /// This method is searching the newDevices list and finds the devices that are no longer available since the last check. The found devices are removed from the main list.
        /// </summary>
        /// <returns>Returns true if a device is removed, false otherwise</returns>
        protected bool RemoveNoLongerVisibleDevices()
        {
            bool removed = false;

            auxDevices.Clear();

            // We make a search to see if a device is no longer visible, if so, we need to remove it from the list.
            foreach (BluetoothDeviceInfo device in devices)
            {
                if (!newDevices.Contains(device))
                {
                    auxDevices.Add(device);
                }
            }

            foreach (BluetoothDeviceInfo device in auxDevices)
            {
                devices.Remove(device);
                removed = true;
            }

            return removed;
        }

        /// <summary>
        /// This method will be implemented by the classes that inherit this class and will be called when the timer.Elapsed event will be raised.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected abstract void timer_Elapsed(object sender, ElapsedEventArgs e);
    }
}
