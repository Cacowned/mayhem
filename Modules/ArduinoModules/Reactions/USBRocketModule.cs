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
using UsbLibrary;
using DefaultModules.Wpf;
using System.Windows.Threading;


namespace ArduinoModules.Reactions
{


  

    
    public class USBRocketLauncher
    {
        
        private string VID = "vid_0a81";
        private string PID = "pid_0701";
        private int vid = 0x0a81;
        private int pid = 0x0701;

        private string[] devices;
       

        UsbHidPort mDevice;
        bool mConnected = false;
        byte[] mCmdData = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public USBRocketLauncher()
        {
            mDevice = new UsbHidPort();
            mDevice.VendorId = vid;
            mDevice.ProductId = pid;
            mDevice.CheckDevicePresent();

           
           
            mDevice.SpecifiedDevice.DataRecieved += new DataRecievedEventHandler(e_DataReceived);
            mConnected = true;

            mCmdData[1] = 16;
            mDevice.SpecifiedDevice.SendData(mCmdData);

            Dispatcher.CurrentDispatcher.VerifyAccess();
            Dispatcher.Run();

        }

        void e_DataReceived(object o, DataRecievedEventArgs e)
        {
            
            int fireState = e.data[1] & (1 << 7);
            Logger.WriteLine("FireState: "+fireState);
        }

     



      

     

        ~USBRocketLauncher()
        {
            mDevice.Dispose();
        }
    }







    [DataContract]
    [MayhemModule("USB Rocket Launcher", "Launches USB Rocket")]
    public class USBRocketModule : ReactionBase, IWpfConfigurable
    {
        public IWpfConfiguration ConfigurationControl
        {
            get {
                USBRocketLauncher launcher = new USBRocketLauncher();
                return new DebugMessageConfig("foo");
            }
        }

        public override void Perform()
        {
            //throw new NotImplementedException();
            // foo
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            throw new NotImplementedException();
        }
    }
}
