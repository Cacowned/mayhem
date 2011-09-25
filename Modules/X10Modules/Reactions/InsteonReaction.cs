﻿/*
 * InsteonReaction.cs
 * 
 * Mayhem Reaction that can send command to Insteon Devices
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */


// TODO: SERIALIZATION 


using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using MayhemSerial;
using X10Modules.Insteon;
using X10Modules.Wpf;
using System;

namespace X10Modules.Reactions
{
    [DataContract]
    [MayhemModule("InsteonReaction", "Triggers Insteon Commands")]
    public class InsteonReaction : ReactionBase, IWpfConfigurable
    {
        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.instance;

        [DataMember]
        private byte[] device_address = new byte[3];

        [DataMember]
        private InsteonStandardMessage command = null;

        [DataMember]
        private string portName = string.Empty;

        
        private InsteonController insteonController = null;


        [OnDeserialized]
        public void OnLoad(StreamingContext s)
        {
            // basically reintialize the serial connection
            serial = MayhemSerialPortMgr.instance;
            // TODO evaluate is Serial.findInsteonDevices may be 
            if (serial.getInsteonPortNames().Keys.Contains(portName))
            {
                insteonController = new InsteonController(portName);
            }
            SetConfigString();
        }

        public override void Perform()
        {
            //throw new NotImplementedException();
            insteonController.SendStandardMsg(command);
        }

        public WpfConfiguration ConfigurationControl
        {
            get 
            {
                //throw new NotImplementedException();
                Logger.WriteLine("ConfigurationControl");
                return new InsteonReactionConfig();
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            InsteonReactionConfig c = configurationControl as InsteonReactionConfig;
            device_address = c.selected_device_address;
            portName = c.selected_portname;
            byte command_byte = c.selected_command;
            command = new InsteonStandardMessage(device_address, command_byte);
            insteonController = new InsteonController(portName);
            SetConfigString();
        }

        
        public override void SetConfigString()
        {
            string config = String.Format("Device: {0:x2}:{1:x2}:{2:x2}", device_address[0], device_address[1], device_address[2]);
            if (command != null)
                config += ", Command: " + command.ToString();
            ConfigString = config;
        }
    }
}
