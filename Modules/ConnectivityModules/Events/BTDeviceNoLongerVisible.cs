﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using MayhemCore;

namespace ConnectivityModule.Events
{
    /// <summary>
    /// A class that detects when a bluetooth device is no longer visible.
    /// </summary>
    [MayhemModule("Bluetooth: Device No Longer Visible", "Triggers when a device is no longer visible")]
    public class BTDeviceNoLongerVisible : BTDeviceBaseClass
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            try
            {
                // Calling this method will return a list of the currently visible devices.
                // Note that this method takes up to 10 seconds to complete, in order to have accurate results.
                newDevices = bluetoothClient.DiscoverDevices(10, false, false, false, true).ToList();

                if (devices == null)
                {
                    // If it's the first time this event is triggered we get the list of the bluetooth devices and we set the timer.
                    MakeFirstListOfDevices();
                }
                else
                {
                    FindNewDevices();

                    if (RemoveNoLongerVisibleDevices())
                    {
                        // If a device is no longer visible we trigger this event.
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
