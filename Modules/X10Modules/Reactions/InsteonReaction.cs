/*
 * InsteonReaction.cs
 * 
 * Mayhem Reaction that can send command to Insteon Devices
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */

using System;
using System.Linq;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemSerial;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using X10Modules.Insteon;
using X10Modules.Wpf;

namespace X10Modules.Reactions
{
    [DataContract]
    [MayhemModule("InsteonReaction", "Triggers Insteon Commands")]
    public class InsteonReaction : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private byte[] device_address;

        [DataMember]
        private InsteonStandardMessage command;

        [DataMember]
        private string portName;

        private MayhemSerialPortMgr serial;

        private InsteonController insteonController = null;

        protected override void OnLoadDefaults()
        {
            device_address = new byte[3];
            command = null;
            portName = string.Empty;
        }

        protected override void OnAfterLoad()
        {
            serial = MayhemSerialPortMgr.instance;
            // TODO evaluate is Serial.findInsteonDevices may be 
            if (serial.getInsteonPortNames().Keys.Contains(portName))
            {
                insteonController = InsteonController.ControllerForPortName(portName); //new InsteonController(portName);
            }
        }

        public override void Perform()
        {
            insteonController.SendStandardMsg(command);
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
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
            insteonController = InsteonController.ControllerForPortName(portName);
        }


        public string GetConfigString()
        {
            string config = String.Format("Device: {0:x2}:{1:x2}:{2:x2}", device_address[0], device_address[1], device_address[2]);
            if (command != null)
                config += ", Command: " + command.ToString();
            return config;
        }
    }
}
