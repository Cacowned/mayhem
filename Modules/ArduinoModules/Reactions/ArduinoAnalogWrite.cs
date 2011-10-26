using System.Runtime.Serialization;
using MayhemCore;
using ArduinoModules.Wpf.Helpers;
using System.Collections.Generic;
using ArduinoModules.Firmata;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
namespace ArduinoModules.Reactions
{
    /// <summary>
    /// Writes PWM to analog-out pins of the Arduino
    /// </summary>
    /// 
    [DataContract]
    [MayhemModule("Arduino Digital Write", "Writes logic values to a set of digital pins on the Arduino.")]
    public class ArduinoAnalogWrite : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private List<AnalogWriteItem> writePins;

        [DataMember]
        private string arduinoPortName;

        private ArduinoFirmata arduino = null;

        protected override void OnLoadDefaults()
        {
            //base.OnLoadDefaults();
            writePins = new List<AnalogWriteItem>();
            arduinoPortName = string.Empty;
        }

        protected override void OnAfterLoad()
        {
            if (arduinoPortName != string.Empty)
            {
                arduino = ArduinoFirmata.InstanceForPortname(arduinoPortName);

                foreach (AnalogWriteItem pin in writePins)
                {
                    arduino.SetPinMode(pin.PinId, PinMode.PWM);
                }
            }
        }

        public override void Perform()
        {
            if (arduino != null)
            {
                foreach (AnalogWriteItem p in writePins)
                {
                    arduino.AnalogWrite(p.PinId, p.AnalogWriteValue);
                }
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get { throw new System.NotImplementedException(); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            throw new System.NotImplementedException();
        }

        public string GetConfigString()
        {
            throw new System.NotImplementedException();
        }
    }

}
