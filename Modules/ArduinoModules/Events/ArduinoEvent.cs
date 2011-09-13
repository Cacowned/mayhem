using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemDefaultStyles.UserControls;
using System.Diagnostics;
using ArduinoModules.Wpf;
using System.Runtime.Serialization;
using MayhemSerial;

namespace ArduinoModules.Events
{ 
    [DataContract]
    [MayhemModule("Arduino Event", "**Testing** Detects Pin Changes in Arduino")]
    public class ArduinoEvent : EventBase, IWpfConfigurable
    {
        private const string TAG = "[ArduinoEvent] : ";
        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.instance;

        public IWpfConfiguration ConfigurationControl
        {
            get
            {
                Debug.WriteLine(TAG + "ConfigurationControl");

                // TODO
  
                ArduinoEventConfig config = new ArduinoEventConfig();           
                return config;
            }
        }



        public void OnSaved(IWpfConfiguration configurationControl)
        {
            // TODO
            //throw new NotImplementedException();
        }
    }
}
