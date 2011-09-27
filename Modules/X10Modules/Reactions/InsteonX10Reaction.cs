﻿/*
 * InsteonX10Reaction.cs
 * 
 * X10  Mayhem Reaction for Insteon Modules -- can be used to control X10 devices on the power line
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * 
 * Author: Sven Kratz
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using MayhemSerial;
using System.Diagnostics;
using X10Modules.Wpf;
using X10Modules.Insteon;
using System.Threading;

namespace X10Modules.Reactions
{
    [DataContract]
    [MayhemModule("X10Reaction", "Triggers X10 Commands")]
    public class InsteonX10Reaction : ReactionBase, IWpfConfigurable
    {
        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.instance;

        [DataMember]
        private X10HouseCode houseCode = X10HouseCode.A;

        [DataMember]
        private X10UnitCode unitCode = X10UnitCode.U1;

        [DataMember]
        private X10CommandCode commandCode = X10CommandCode.ON;

        [DataMember]
        private string serialPortName = string.Empty;

        private X10Controller x10Controller = null;

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                Logger.WriteLine("ConfigurationControl");
                InsteonX10ReactionConfig config = new InsteonX10ReactionConfig();
                return config;
            }
        }

        [OnDeserialized]
        public void OnLoad(StreamingContext s)
        {
            // basically reintialize the serial connection
            serial = MayhemSerialPortMgr.instance;
            if (serial.PortExists(this.serialPortName))
            {
                x10Controller =  X10Controller.ControllerForPortName(serialPortName);
            }
            SetConfigString();
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            Logger.WriteLine("OnSaved");
            // TODO
            InsteonX10ReactionConfig c = configurationControl as InsteonX10ReactionConfig;

            houseCode = c.selected_housecode;
            unitCode = c.selected_unitcode;
            commandCode = c.selected_commandcode;
            serialPortName = "" + c.selected_portName;
            if (x10Controller != null)
            {
                x10Controller.Dispose();
            }
            x10Controller =  X10Controller.ControllerForPortName(serialPortName);
            SetConfigString();
        }

        public override void Perform()
        {
            Logger.WriteLine("Perform()");
            x10Controller.X10SendCommand(houseCode, unitCode, commandCode);
        }

        public override void SetConfigString()
        {
            string config = "House: " + houseCode.ToString() + ", Unit: " + unitCode.ToString() + ", Command: " + commandCode.ToString();
            ConfigString = config; 
        }

    }
}
