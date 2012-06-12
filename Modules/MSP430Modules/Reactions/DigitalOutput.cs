using System;
using System.IO.Ports;
using System.Runtime.Serialization;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using MSP430Modules;
using MSP430Modules.Wpf;
using SerialManager;

namespace MSP430Modules.Reactions
{
    [DataContract]
    [MayhemModule("MSP430: Digital Output", "Triggers a digital output")]
    public class DigitalOutput : ReactionBase, IWpfConfigurable
    {
        private SerialPortManager manager;

        [DataMember]
        private SerialSettings settings;

        [DataMember]
        private int index;
        
        [DataMember]
        private string port;
        
        [DataMember]
        private DigitalOutputType outputType;

        [DataMember]
        private string phrase;        

        protected override void OnLoadDefaults()
        {
            index = 0;            
            outputType = DigitalOutputType.Toggle;
            port = "COM1";
            this.phrase = string.Empty;
            this.settings = new SerialSettings(9600, Parity.Even, StopBits.One, 8);          
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new MSP430DigitalOutputConfig(index, outputType, port); }
        }

        public void OnSaved(WpfConfiguration ConfigurationControl)
        {
            var config = ConfigurationControl as MSP430DigitalOutputConfig;

            index = config.Index;                       
            outputType = config.OutputType;
            port = config.Port;
        }

        public string GetConfigString()
        {
            //GetConfigString appears below the event/reaction in the Mayhem UI

            //Action to take
            string action = string.Empty;
            switch (outputType)
            {
                case DigitalOutputType.Toggle: action = "Toggle";
                    break;
                case DigitalOutputType.On: action = "Turn On";
                    break;
                case DigitalOutputType.Off: action = "Turn Off";
                    break;
            }

            //Index to show in the string (ie. P1.0, P1.4, etc.)
            string indexName = string.Empty;
            switch (index)
            {
                case 0: indexName = "P1.0";
                    break;
                case 1: indexName = "P1.4";
                    break;
                case 2: indexName = "P1.5";
                    break;
                case 3: indexName = "P1.6";
                    break;
                case 4: indexName = "P1.7";
                    break;
                case 5: indexName = "P2.0";
                    break;
            }
            
            // Return string
            return action + " " + indexName + " on " + port;

            //switch (port)
            //{
            //    case "COM1": type = "first com port";
            //        break;
            //    case "COM2": type = "first com port";
            //        break;
            //    case "COM3": type = "first com port";
            //        break;
            //}
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
            if (outputType.Equals(DigitalOutputType.Toggle))
            {
                if (index.Equals(0)) { byte[] sendByte = { 0x00 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(1)) { byte[] sendByte = { 0x01 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(2)) { byte[] sendByte = { 0x02 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(3)) { byte[] sendByte = { 0x03 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(4)) { byte[] sendByte = { 0x04 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(5)) { byte[] sendByte = { 0x05 }; this.manager.Write(this.port, sendByte, 1); }
            }

            if (outputType.Equals(DigitalOutputType.On))
            {
                if (index.Equals(0)) { byte[] sendByte = { 0x10 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(1)) { byte[] sendByte = { 0x11 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(2)) { byte[] sendByte = { 0x12 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(3)) { byte[] sendByte = { 0x13 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(4)) { byte[] sendByte = { 0x14 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(5)) { byte[] sendByte = { 0x15 }; this.manager.Write(this.port, sendByte, 1); }
            }

            if (outputType.Equals(DigitalOutputType.Off))
            {
                if (index.Equals(0)) { byte[] sendByte = { 0x20 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(1)) { byte[] sendByte = { 0x21 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(2)) { byte[] sendByte = { 0x22 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(3)) { byte[] sendByte = { 0x23 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(4)) { byte[] sendByte = { 0x24 }; this.manager.Write(this.port, sendByte, 1); }
                if (index.Equals(5)) { byte[] sendByte = { 0x25 }; this.manager.Write(this.port, sendByte, 1); }
            }

            //else
            //{
            //    ErrorLog.AddError(ErrorType.Failure, "MSP430 module is not attached");
            //}
        }
    }

    // What action we should take on the digital output
    public enum DigitalOutputType
    {
        Toggle, 
        On,
        Off
    }
}
