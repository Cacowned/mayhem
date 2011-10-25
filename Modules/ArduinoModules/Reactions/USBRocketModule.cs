using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using MayhemWpf.ModuleTypes;
using USBHIDDRIVER;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO;



namespace ArduinoModules.Reactions
{


   //byte[] command = new byte[] { 0x02, cmd, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

    
    public class USBRocketLauncher
    {
        
      //  private string VID = "vid_2123";
      //  private string PID = "pid_1010";
        private int VID = 0x2123;
        private int PID = 0x1010;

        private static bool attached = false;

        private static USBInterface usb;

        // sure, we could find this out the hard way using HID, but trust me, it's 22
        private const int REPORT_LENGTH = 22;

        // read/write handle to the device
		private SafeFileHandle mHandle;

        // a pretty .NET stream to read/write from/to
        private FileStream mStream;

        // report buffer
        private  byte[] mBuff = new byte[REPORT_LENGTH];

        // use a different method to write reports
        private bool mAltWriteMethod;

        private void SendCmd(byte cmd)
        {       
            byte[] command = new byte[] { 0x02, cmd, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            mBuff = command;
            mAltWriteMethod = true;
            WriteReport();
        }

       
        void device_Removed()
        {
         
        }

        void device_Inserted()
        {
           
        }

        public USBRocketLauncher()
        {
       
			int index = 0;
			bool found = false;
			Guid guid;



			// get the GUID of the HID class
			HIDImports.HidD_GetHidGuid(out guid);

			// get a handle to all devices that are part of the HID class
			// Fun fact:  DIGCF_PRESENT worked on my machine just fine.  I reinstalled Vista, and now it no longer finds the Wiimote with that parameter enabled...
			IntPtr hDevInfo = HIDImports.SetupDiGetClassDevs(ref guid, null, IntPtr.Zero, HIDImports.DIGCF_DEVICEINTERFACE);// | HIDImports.DIGCF_PRESENT);

			// create a new interface data struct and initialize its size
			HIDImports.SP_DEVICE_INTERFACE_DATA diData = new HIDImports.SP_DEVICE_INTERFACE_DATA();
			diData.cbSize = Marshal.SizeOf(diData);

			// get a device interface to a single device (enumerate all devices)
			while(HIDImports.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref guid, index, ref diData))
			{
				UInt32 size;

				// get the buffer size for this device detail instance (returned in the size parameter)
				HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, IntPtr.Zero, 0, out size, IntPtr.Zero);

				// create a detail struct and set its size
				HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA diDetail = new HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA();

				// yeah, yeah...well, see, on Win x86, cbSize must be 5 for some reason.  On x64, apparently 8 is what it wants.
				// someday I should figure this out.  Thanks to Paul Miller on this...
				diDetail.cbSize = (uint)(IntPtr.Size == 8 ? 8 : 5);

				// actually get the detail struct
				if(HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, ref diDetail, size, out size, IntPtr.Zero))
				{
					//Logger.WriteLine(index + " " + diDetail.DevicePath + " " + Marshal.GetLastWin32Error());

					// open a read/write handle to our device using the DevicePath returned
					mHandle = HIDImports.CreateFile(diDetail.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);

					// create an attributes struct and initialize the size
					HIDImports.HIDD_ATTRIBUTES attrib = new HIDImports.HIDD_ATTRIBUTES();
					attrib.Size = Marshal.SizeOf(attrib);

					// get the attributes of the current device
					if(HIDImports.HidD_GetAttributes(mHandle.DangerousGetHandle(), ref attrib))
					{
						// if the vendor and product IDs match up
						if(attrib.VendorID == VID && attrib.ProductID == PID)
						{
							Logger.WriteLine("Found it!");
							found = true;

							// create a nice .NET FileStream wrapping the handle above
							mStream = new FileStream(mHandle, FileAccess.ReadWrite, REPORT_LENGTH, true);

							// start an async read operation on it
                            //BeginAsyncRead();

                            //// read the calibration info from the controller
                            //try
                            //{
                            //    ReadCalibration();
                            //}
                            //catch
                            //{
                            //    // if we fail above, try the alternate HID writes
                            //    mAltWriteMethod = true;
                            //    ReadCalibration();
                            //}

                            //// force a status check to get the state of any extensions plugged in at startup
                            //GetStatus();

							break;
						}
						else
						{
							// otherwise this isn't the controller, so close up the file handle
							mHandle.Close();
						}
					}
				}
				else
				{
					// failed to get the detail struct
					//throw new WiimoteException("SetupDiGetDeviceInterfaceDetail failed on index " + index);
				}

				// move to the next device
				index++;
			}

			// clean up our list
			HIDImports.SetupDiDestroyDeviceInfoList(hDevInfo);

			// if we didn't find a Wiimote, throw an exception
			//if(!found)
				//throw new WiimoteException("Wiimote not found in HID device list.");
		
        }

        /// <summary>
        /// Initialize the report data buffer
        /// </summary>
        private void ClearReport()
        {
            Array.Clear(mBuff, 0, REPORT_LENGTH);
        }

        /// <summary>
        /// Write a report to the Wiimote
        /// </summary>
        private void WriteReport()
        {
            if (mAltWriteMethod)
                HIDImports.HidD_SetOutputReport(this.mHandle.DangerousGetHandle(), mBuff, (uint)mBuff.Length);
            else if (mStream != null)
                mStream.Write(mBuff, 0, mBuff.Length);

            Thread.Sleep(100);
        }

        /// <summary>
        /// Start reading asynchronously from the controller
        /// </summary>
        private void BeginAsyncRead()
        {
            // if the stream is valid and ready
            if (mStream != null && mStream.CanRead)
            {
                // setup the read and the callback
                byte[] buff = new byte[REPORT_LENGTH];
                mStream.BeginRead(buff, 0, REPORT_LENGTH, new AsyncCallback(OnReadData), buff);
            }
        }

        /// <summary>
        /// Callback when data is ready to be processed
        /// </summary>
        /// <param name="ar">State information for the callback</param>
        private void OnReadData(IAsyncResult ar)
        {
            // grab the byte buffer
            byte[] buff = (byte[])ar.AsyncState;

            try
            {
                // end the current read
                mStream.EndRead(ar);

                // parse it
                //if (ParseInputReport(buff))
                //{
                //    // post an event
                //    if (WiimoteChanged != null)
                //        WiimoteChanged(this, new WiimoteChangedEventArgs(mWiimoteState));
                //}

                // start reading again
                BeginAsyncRead();
            }
            catch (OperationCanceledException)
            {
                Logger.WriteLine("OperationCanceledException");
            }
        }

        private void bufferEvent(object sender, EventArgs e)
        {
            Logger.Write("USB Buffer Event");
            usb.stopRead();

        }

        public void testSend()
        {
            Logger.WriteLine("Sending!");
            SendCmd(0x04);
            //device.Read(read);
           
            SendCmd(0x20);
            Logger.WriteLine("Done!");
   
        }

        
    
    }

    [DataContract]
    [MayhemModule("USB Rocket Launcher", "Launches USB Rocket")]
    
    public class USBRocketModule : ReactionBase
    {
        //public IWpfConfiguration ConfigurationControl
        //{
        //    get {
        //        USBRocketLauncher launcher = new USBRocketLauncher();
        //        return new DebugMessageConfig("foo");
        //    }
        //}

        private USBRocketLauncher   launcher = new USBRocketLauncher();

     
        public override void Perform()
        {
            //throw new NotImplementedException();
            // foo
            if (launcher == null)
                launcher = new USBRocketLauncher();
            launcher.testSend();
        }

        //public void OnSaved(IWpfConfiguration configurationControl)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
