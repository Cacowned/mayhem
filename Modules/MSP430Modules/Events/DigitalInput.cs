using System;
using System.IO.Ports;
using System.Runtime.Serialization;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using MSP430Modules.Wpf;
using SerialManager;

namespace MSP430Modules.Events
{
    [DataContract]
    [MayhemModule("MSP430: Digital Input", "Listens to a digital input")]
    public class DigitalInput : EventBase, IWpfConfigurable
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
        private DigitalInputType inputType;

        [DataMember]
        private string phrase;

        protected override void OnLoadDefaults()
        {
            index = 0;
            inputType = DigitalInputType.Toggle;
            portName = "";
            this.phrase = string.Empty;
            this.settings = new SerialSettings(9600, Parity.Even, StopBits.One, 8);
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new MSP430DigitalInputConfig(index, inputType, port, portName); }
        }

        public void OnSaved(WpfConfiguration ConfigurationControl)
        {
            var config = ConfigurationControl as MSP430DigitalInputConfig;

            index = config.Index;
            inputType = config.InputType;
            port = config.Port;
            portName = config.PortName;
        }

        public string GetConfigString()
        {
            //GetConfigString appears below the event/reaction in the Mayhem UI

            // Action to be displayed in the configuration string
            string action = string.Empty;
            switch (inputType)
            {
                case DigitalInputType.Toggle: action = "Toggles";
                    break;
                case DigitalInputType.High: action = "Turns High";
                    break;
                case DigitalInputType.Low: action = "Turns Low";
                    break;
            }

            // Index to show in the string (ie. P1.3, P2.1, etc.)
            string indexName = string.Empty;
            switch (index)
            {
                case 0: indexName = "P1.3";
                    break;
                case 1: indexName = "P2.1";
                    break;
                case 2: indexName = "P2.2";
                    break;
                case 3: indexName = "P2.3";
                    break;
                case 4: indexName = "P2.4";
                    break;
                case 5: indexName = "P2.5";
                    break;
            }

            // COM Port
            // 'port' contains the COM port to display

            // Return the configuration string
            return indexName + " " + action + " on " + port;
        }

        protected override void OnAfterLoad()
        {
            this.manager = SerialPortManager.Instance;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            try
            {
                this.manager.ConnectPort(this.port, this.settings, this.ReceivedData);
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Unable to open COM port. Ensure port is available or device plugged in.");
                e.Cancel = true;
                return;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            try
            {
                this.manager.ReleasePort(this.port, this.ReceivedData);
            }
            catch
            {
                /* Swallow the exception because the only times this will happen is if we
                 * try to close a port that hasn't been opened or if it hasn't been connected with the specified action. 
                 * Aka, it was closed here before it was opened. Nothing we can do, might as well hide it
                 */
            }
        }

        private void ReceivedData(byte[] bytes, int numBytes)
        {
            // Get bytes received from the serial port
            string byteReceived = Encoding.UTF8.GetString(bytes);

            // Check toggle conditions from firmware
            if (inputType.Equals(DigitalInputType.Toggle))
            {
                if (index.Equals(0) && (byteReceived.Equals("P") || byteReceived.Equals("`"))) Trigger(); //0x50 OR 0x60
                if (index.Equals(1) && (byteReceived.Equals("Q") || byteReceived.Equals("a"))) Trigger(); //0x51 OR 0x61
                if (index.Equals(2) && (byteReceived.Equals("R") || byteReceived.Equals("b"))) Trigger(); //0x52 OR 0x62
                if (index.Equals(3) && (byteReceived.Equals("S") || byteReceived.Equals("c"))) Trigger(); //0x53 OR 0x63
                if (index.Equals(4) && (byteReceived.Equals("T") || byteReceived.Equals("d"))) Trigger(); //0x54 OR 0x64
                if (index.Equals(5) && (byteReceived.Equals("U") || byteReceived.Equals("e"))) Trigger(); //0x55 OR 0x65                
            }

            // Check turns high conditions from firmware
            if (inputType.Equals(DigitalInputType.High))
            {
                if (index.Equals(0) && byteReceived.Equals("P")) Trigger(); //0x50
                if (index.Equals(1) && byteReceived.Equals("Q")) Trigger(); //0x51
                if (index.Equals(2) && byteReceived.Equals("R")) Trigger(); //0x52
                if (index.Equals(3) && byteReceived.Equals("S")) Trigger(); //0x53
                if (index.Equals(4) && byteReceived.Equals("T")) Trigger(); //0x54
                if (index.Equals(5) && byteReceived.Equals("U")) Trigger(); //0x55            
            }

            // Check turns low conditions from firmware
            if (inputType.Equals(DigitalInputType.Low))
            {
                if (index.Equals(0) && byteReceived.Equals("`")) Trigger(); //0x60
                if (index.Equals(1) && byteReceived.Equals("a")) Trigger(); //0x61
                if (index.Equals(2) && byteReceived.Equals("b")) Trigger(); //0x62
                if (index.Equals(3) && byteReceived.Equals("c")) Trigger(); //0x63
                if (index.Equals(4) && byteReceived.Equals("d")) Trigger(); //0x64
                if (index.Equals(5) && byteReceived.Equals("e")) Trigger(); //0x65
            }
        }
    }

    public enum DigitalInputType
    {
        Toggle,
        High,
        Low
    }
}