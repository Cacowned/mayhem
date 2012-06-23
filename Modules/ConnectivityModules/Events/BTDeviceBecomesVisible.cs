﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using MayhemCore;

namespace ConnectivityModule.Events
{
    /// <summary>
    /// A class that detects when a bluetooth device becomes visible.
    /// </summary>
    [MayhemModule("Bluetooth: New Device Visible", "Triggers when a device becomes visible")]
    public class BTDeviceBecomesVisible : BTDeviceBaseClass
    {
        /// <summary>
        /// This method is called when the timer.Elapsed event is raised and checks if a bluetooth device became visible from the last check.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Stopping the timer so this event will not be triggered again until the method exits.
            timer.Stop();

            try
            {
                // Calling this method will return a list of the currently visible devices.
                // Note that this method takes up to 10 seconds to complete, in order to have accurate results.
                newDevices = bluetoothClient.DiscoverDevices(10, false, false, false, true).ToList();

                if (devices == null)
                {
                    // If it's the first time this event is triggered we get the list of the devices and we set the timer.
                    MakeFirstListOfDevices();
                }
                else
                {
                    if (FindNewDevices())
                    {
                        Trigger();
                    }

                    RemoveNoLongerVisibleDevices();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.BT_ErrorMonitoringBluetooth);
                Logger.Write(ex);
            }

            // Starting the timer.
            timer.Start();
        }
    }
}
