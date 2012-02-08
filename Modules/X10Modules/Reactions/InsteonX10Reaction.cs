using System.Runtime.Serialization;
using MayhemCore;
using MayhemSerial;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using X10Modules.Insteon;
using X10Modules.Wpf;

namespace X10Modules.Reactions
{
    /// <summary>
    /// X10 Mayhem Reaction for Insteon Modules -- can be used to control X10 devices on the power line
    /// </summary>
    [DataContract]
    [MayhemModule("X10Reaction", "Triggers X10 Commands")]
    public class InsteonX10Reaction : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private X10HouseCode houseCode;

        [DataMember]
        private X10UnitCode unitCode;

        [DataMember]
        private X10CommandCode commandCode;

        [DataMember]
        private string serialPortName;

        private MayhemSerialPortMgr serial;

        private X10Controller x10Controller;

        protected override void OnLoadDefaults()
        {
            houseCode = X10HouseCode.A;
            unitCode = X10UnitCode.U1;
            commandCode = X10CommandCode.On;
            serialPortName = string.Empty;
        }

        protected override void OnAfterLoad()
        {
            serial = MayhemSerialPortMgr.Instance;
            if (serial.PortExists(serialPortName))
            {
                x10Controller = X10Controller.ControllerForPortName(serialPortName);
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            Logger.WriteLine("OnSaved");

            // TODO
            InsteonX10ReactionConfig c = configurationControl as InsteonX10ReactionConfig;

            houseCode = c.SelectedHousecode;
            unitCode = c.SelectedUnitcode;
            commandCode = c.SelectedCommandcode;
            serialPortName = c.SelectedPortName;
            if (x10Controller != null)
            {
                x10Controller.Dispose();
            }

            x10Controller = X10Controller.ControllerForPortName(serialPortName);
        }

        public override void Perform()
        {
            Logger.WriteLine("Perform()");
            x10Controller.X10SendCommand(houseCode, unitCode, commandCode);
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                Logger.WriteLine("ConfigurationControl");
                InsteonX10ReactionConfig config = new InsteonX10ReactionConfig();
                return config;
            }
        }

        public string GetConfigString()
        {
            return "House: " + houseCode.ToString() + ", Unit: " + unitCode.ToString() + ", Command: " + commandCode.ToString();
        }
    }
}
