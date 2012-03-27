using System.IO.Ports;
using System.Runtime.Serialization;
using System.Text;
using MayhemCore;
using MayhemSerial;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using SerialModules.Wpf;

namespace SerialModules.Events
{
    [DataContract]
    [MayhemModule("Serial Listen", "Listens Serially")]
    public class SerialListen : EventBase, IWpfConfigurable
    {
        private SerialPortManager manager;

        [DataMember]
        private SerialSettings settings;

        [DataMember]
        private string port;

        [DataMember]
        private string phrase;

        protected override void OnLoadDefaults()
        {
            this.port = "COM1";
            this.phrase = string.Empty;
            this.settings = new SerialSettings(9600, Parity.Even, StopBits.One, 8);
        }

        protected override void OnAfterLoad()
        {
            this.manager = SerialPortManager.Instance;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            this.manager.ConnectPort(this.port, this.settings, this.RecievedData);
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            this.manager.ReleasePort(this.port, this.RecievedData);
        }

        private void RecievedData(byte[] bytes, int numBytes)
        {
            string str = Encoding.UTF8.GetString(bytes);

            if (str == this.phrase)
            {
                Trigger();
            }
        }

        #region IWpfConfigurable
        public WpfConfiguration ConfigurationControl
        {
            get
            {
                return new SerialListenConfig(this.port, this.settings, this.phrase);
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as SerialListenConfig;
            this.port = config.Selector.PortName;
            this.settings = config.Selector.Settings;

            this.phrase = config.Phrase;
        }

        public string GetConfigString()
        {
            return string.Format("Listening for '{0}' on {1}", this.phrase, this.port);
        }
        #endregion
    }
}
