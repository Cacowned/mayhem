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
using Indy.Rocket.Core;

namespace ArduinoModules.Reactions
{

    using System;
    using System.Threading;
    using UsbLibrary;
  

    
    public class USBRocketLauncher
    {
        
        private string VID = "vid_0a81";
        private string PID = "pid_0701";
        private int vid = 0x0a81;
        private int pid = 0x0701;

        private string[] devices;
        private Rocket r = null;

        UsbHidPort mDevice = new UsbHidPort();
        bool mConnected = false;
        byte[] mCmdData = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public USBRocketLauncher()
        {

            mDevice.VendorId = vid;
            mDevice.ProductId = pid;
            mDevice.CheckDevicePresent();
            
            mDevice.SpecifiedDevice.DataRecieved += new DataRecievedEventHandler(e_DataReceived);
            mConnected = true;

            mCmdData[1] = 16;
            mDevice.SpecifiedDevice.SendData(mCmdData);
            

            /*
            r = new Rocket(vid, pid);
            r.Connect();
            Thread.Sleep(50);
            if (r.Connected)
            {
                r.MissileFired += new MissileFiredDelegate(r_MissileFired);
                Logger.WriteLine("Rocket Connected");
                r.FireOnce();
            }*/
              
           

        }

        void e_DataReceived(object o, DataRecievedEventArgs e)
        {
            Logger.WriteLine("data");
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
                return new IWpfConfiguration();
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
