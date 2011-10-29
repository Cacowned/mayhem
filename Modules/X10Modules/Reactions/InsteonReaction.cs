using System;
using System.Linq;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemSerial;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using X10Modules.Insteon;
using X10Modules.Insteon.InsteonCommands;
using X10Modules.Wpf;

namespace X10Modules.Reactions
{
    /// <summary>
    /// Mayhem Reaction that can send command to Insteon Devices
    /// </summary>
    [DataContract]
    [MayhemModule("InsteonReaction", "Triggers Insteon Commands")]
    public class InsteonReaction : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private byte[] deviceAddress;

        [DataMember]
        private InsteonStandardMessage command;

        [DataMember]
        private string portName;

        private MayhemSerialPortMgr serial;

        private InsteonController insteonController = null;

        protected override void OnLoadDefaults()
        {
            deviceAddress = new byte[3];
            command = null;
            portName = string.Empty;
        }

        protected override void OnAfterLoad()
        {
            serial = MayhemSerialPortMgr.Instance;
            // TODO evaluate is Serial.findInsteonDevices may be 
            if (serial.GetInsteonPortNames(new InsteonUsbModemSerialSettings()).Keys.Contains(portName))
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
            deviceAddress = c.SelectedDeviceAddress;
            portName = c.SelectedPortname;
            byte commandByte = c.SelectedCommand;
            command = new InsteonStandardMessage(deviceAddress, commandByte);
            insteonController = InsteonController.ControllerForPortName(portName);
        }


        public string GetConfigString()
        {
            string config = String.Format("Device: {0:x2}:{1:x2}:{2:x2}", deviceAddress[0], deviceAddress[1], deviceAddress[2]);
            if (command != null)
                config += ", Command: " + command.ToString();
            return config;
        }
    }
}
