using System;
using System.IO.Ports;
using System.Runtime.Serialization;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using ArduinoModules;
using ArduinoModules.Wpf;
using SerialManager;

namespace ArduinoModules.Events
{
    [DataContract]
    [MayhemModule("Arduino: Digital Input", "Listens to a digital input")]
    public class DigitalInput : EventBase, IWpfConfigurable
    {
        private SerialPortManager manager;

        [DataMember]
        private SerialSettings settings;

        [DataMember]
        private int index;
        
        [DataMember]
        private string port;

        [DataMember]
        private DigitalInputType inputType;
        
        [DataMember]
        private string phrase;

        protected override void OnLoadDefaults()
        {
            index = 0;
            inputType = DigitalInputType.Toggle;
            port = "COM1";
            this.phrase = string.Empty;
            this.settings = new SerialSettings(9600, Parity.Even, StopBits.One, 8);                     
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new ArduinoDigitalInputConfig(index, inputType, port); }
        }

        public void OnSaved(WpfConfiguration ConfigurationControl)
        {
            var config = ConfigurationControl as ArduinoDigitalInputConfig;

            index = config.Index;
            inputType = config.InputType;
            port = config.Port;
        }

        public string GetConfigString()
        {
            //GetConfigString appears below the event/reaction in the Mayhem UI

            //Action to take
            string action = string.Empty;
            switch (inputType)
            {
                case DigitalInputType.Toggle: action = "Toggles";
                    break;
                case DigitalInputType.On: action = "Turns On";
                    break;
                case DigitalInputType.Off: action = "Turns Off";
                    break;
            }

            //Index to show in the string (ie. P1.3, P2.1, etc.)
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

            // Return string
            return indexName + " " + action + " on " + port;

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
                this.manager.ConnectPort(this.port, this.settings, this.ReceivedData);
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
           

             
                string str = Encoding.UTF8.GetString(bytes);

                if (str == "1")
                {
                    Trigger();
                }
           
            
            
            
            
            
            

            //if (inputType.Equals(DigitalInputType.Toggle))
            //{
            //    if (index.Equals(0) && byteReceived.Equals("@")) Trigger(); //0x40
            //    if (index.Equals(1) && byteReceived.Equals("A")) Trigger(); //0x41
            //    if (index.Equals(2) && byteReceived.Equals("B")) Trigger(); //0x42
            //    if (index.Equals(3) && byteReceived.Equals("C")) Trigger(); //0x43
            //    if (index.Equals(4) && byteReceived.Equals("D")) Trigger(); //0x44
            //    if (index.Equals(5) && byteReceived.Equals("E")) Trigger(); //0x45
            //}

            //if (inputType.Equals(DigitalInputType.On))
            //{
            //    if (index.Equals(0) && byteReceived.Equals("P")) Trigger(); //0x50
            //    if (index.Equals(1) && byteReceived.Equals("Q")) Trigger(); //0x51
            //    if (index.Equals(2) && byteReceived.Equals("R")) Trigger(); //0x52
            //    if (index.Equals(3) && byteReceived.Equals("S")) Trigger(); //0x53
            //    if (index.Equals(4) && byteReceived.Equals("T")) Trigger(); //0x54
            //    if (index.Equals(5) && byteReceived.Equals("U")) Trigger(); //0x55            
            //}

            //if (inputType.Equals(DigitalInputType.Off))
            //{
            //    if (index.Equals(0) && byteReceived.Equals("`")) Trigger(); //0x60
            //    if (index.Equals(1) && byteReceived.Equals("a")) Trigger(); //0x61
            //    if (index.Equals(2) && byteReceived.Equals("b")) Trigger(); //0x62
            //    if (index.Equals(3) && byteReceived.Equals("c")) Trigger(); //0x63
            //    if (index.Equals(4) && byteReceived.Equals("d")) Trigger(); //0x64
            //    if (index.Equals(5) && byteReceived.Equals("e")) Trigger(); //0x65
            //}
        }
    }

    public enum DigitalInputType
    {
        Toggle, 
        On,
        Off
    }
}
