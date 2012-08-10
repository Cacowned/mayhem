using System;
using System.IO.Ports;
using System.Runtime.Serialization;
using ArduinoModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using SerialManager;

namespace ArduinoModules.Reactions
{
    [DataContract]
    [MayhemModule("Arduino: Digital Output", "Triggers a digital output")]
    public class DigitalOutput : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private SerialPortManager manager;

        [DataMember]
        private SerialSettings settings;

        [DataMember]
        private int index;

        [DataMember]
        private string port;

        [DataMember]
        private string portName;

        [DataMember]
        private DigitalOutputType outputType;

        [DataMember]
        private string phrase;

        protected override void OnLoadDefaults()
        {
            index = 0;
            outputType = DigitalOutputType.Toggle;            
            portName = "";
            this.phrase = string.Empty;
            this.settings = new SerialSettings(9600, Parity.Even, StopBits.One, 8);
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new ArduinoDigitalOutputConfig(index, outputType, port, portName); }
        }

        public void OnSaved(WpfConfiguration ConfigurationControl)
        {
            var config = ConfigurationControl as ArduinoDigitalOutputConfig;

            index = config.Index;
            outputType = config.OutputType;
            port = config.Port;
            portName = config.PortName;
        }

        public string GetConfigString()
        {
            //GetConfigString appears below the event/reaction in the Mayhem UI

            // Action to be displayed in the configuration string
            string action = string.Empty;
            switch (outputType)
            {
                case DigitalOutputType.Toggle: action = "Toggle";
                    break;
                case DigitalOutputType.High: action = "Turn High";
                    break;
                case DigitalOutputType.Low: action = "Turn Low";
                    break;
            }

            // Index to show in the string (ie. pin8, pin9, etc.)
            string indexName = string.Empty;
            switch (index)
            {
                case 0: indexName = "Pin8";
                    break;
                case 1: indexName = "Pin9";
                    break;
                case 2: indexName = "Pin10";
                    break;
                case 3: indexName = "Pin11";
                    break;
                case 4: indexName = "Pin12";
                    break;
                case 5: indexName = "Pin13";
                    break;
            }

            // COM Port
            // 'port' contains the COM port to display

            // Return the configuration string
            return action + " " + indexName + " on " + port;
        }

        protected override void OnAfterLoad()
        {
            this.manager = SerialPortManager.Instance;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            try
            {
                this.manager.ConnectPort(this.port, this.settings);
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, ex.Message);
                e.Cancel = true;
                return;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            try
            {
                this.manager.ReleasePort(this.port);
            }
            catch
            {
                /* Swallow the exception because the only times this will happen is if we
                 * try to close a port that hasn't been opened or if it hasn't been connected with the specified action. 
                 * Aka, it was closed here before it was opened. Nothing we can do, might as well hide it
                 */
            }
        }

        public override void Perform()
        {
            // Send toggle trigger to firmware
            if (outputType.Equals(DigitalOutputType.Toggle))
            {
                if (index.Equals(0)) { byte[] sendByte = { 0x00 }; this.manager.Write(this.port, sendByte, 1); }    // pin8
                if (index.Equals(1)) { byte[] sendByte = { 0x01 }; this.manager.Write(this.port, sendByte, 1); }    // pin9
                if (index.Equals(2)) { byte[] sendByte = { 0x02 }; this.manager.Write(this.port, sendByte, 1); }    // pin10
                if (index.Equals(3)) { byte[] sendByte = { 0x03 }; this.manager.Write(this.port, sendByte, 1); }    // pin11
                if (index.Equals(4)) { byte[] sendByte = { 0x04 }; this.manager.Write(this.port, sendByte, 1); }    // pin12
                if (index.Equals(5)) { byte[] sendByte = { 0x05 }; this.manager.Write(this.port, sendByte, 1); }    // pin13
            }

            // Send turn high trigger to firmware
            if (outputType.Equals(DigitalOutputType.High))
            {
                if (index.Equals(0)) { byte[] sendByte = { 0x10 }; this.manager.Write(this.port, sendByte, 1); }    // pin8
                if (index.Equals(1)) { byte[] sendByte = { 0x11 }; this.manager.Write(this.port, sendByte, 1); }    // pin9
                if (index.Equals(2)) { byte[] sendByte = { 0x12 }; this.manager.Write(this.port, sendByte, 1); }    // pin10
                if (index.Equals(3)) { byte[] sendByte = { 0x13 }; this.manager.Write(this.port, sendByte, 1); }    // pin11
                if (index.Equals(4)) { byte[] sendByte = { 0x14 }; this.manager.Write(this.port, sendByte, 1); }    // pin12
                if (index.Equals(5)) { byte[] sendByte = { 0x15 }; this.manager.Write(this.port, sendByte, 1); }    // pin13
            }

            // Send turn low trigger to firmware
            if (outputType.Equals(DigitalOutputType.Low))
            {
                if (index.Equals(0)) { byte[] sendByte = { 0x20 }; this.manager.Write(this.port, sendByte, 1); }    // pin8
                if (index.Equals(1)) { byte[] sendByte = { 0x21 }; this.manager.Write(this.port, sendByte, 1); }    // pin9
                if (index.Equals(2)) { byte[] sendByte = { 0x22 }; this.manager.Write(this.port, sendByte, 1); }    // pin10
                if (index.Equals(3)) { byte[] sendByte = { 0x23 }; this.manager.Write(this.port, sendByte, 1); }    // pin11
                if (index.Equals(4)) { byte[] sendByte = { 0x24 }; this.manager.Write(this.port, sendByte, 1); }    // pin12
                if (index.Equals(5)) { byte[] sendByte = { 0x25 }; this.manager.Write(this.port, sendByte, 1); }    // pin13
            }
        }
    }

    // What action we should take on the digital output
    public enum DigitalOutputType
    {
        Toggle,
        High,
        Low
    }
}