using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Timers;
using ArduinoModules.Firmata;
using ArduinoModules.Wpf;
using ArduinoModules.Wpf.Helpers;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace ArduinoModules.Reactions
{
    [DataContract]
    [MayhemModule("Arduino Digital Write", "Writes logic values to a set of digital pins on the Arduino.")]
    public class ArduinoDigitalWrite : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private List<DigitalWriteItem> writePins;
        [DataMember]
        private string arduinoPortName;

        private ArduinoFirmata arduino = null;

        // ms pulse time. 
        // TODO: evaluate if this may be required to
        // be set by the user
        private const int PulseTime = 20;

        protected override void OnLoadDefaults()
        {
            writePins = new List<DigitalWriteItem>();
            arduinoPortName = string.Empty;
        }

        protected override void OnAfterLoad()
        {
            if (arduinoPortName != string.Empty)
            {
                arduino = ArduinoFirmata.InstanceForPortname(arduinoPortName);

                foreach (DigitalWriteItem pin in writePins)
                {
                    arduino.SetPinMode(pin.PinId, PinMode.OUTPUT); 
                }

            }
        }

        public override void Perform()
        {
            if (arduino != null)
            {
                foreach (DigitalWriteItem p in writePins)
                {
                    if (p.WriteMode == DigitalWriteMode.HIGH)
                    {
                        // pin will be set to HIGH
                        arduino.DigitalWrite(p.PinId, p.SetPinState(1));
                    }
                    else if (p.WriteMode == DigitalWriteMode.LOW)
                    {
                        // pin will be set to LOW
                        arduino.DigitalWrite(p.PinId, p.SetPinState(0));
                    }
                    else if (p.WriteMode == DigitalWriteMode.PULSE_OFF)
                    {
                        // pin will be set to OFF for a short period
                        PulseOff(p);
                    }
                    else if (p.WriteMode == DigitalWriteMode.PULSE_ON)
                    {
                        // pin will be set to ON for a short period
                        PulseOn(p);
                    }
                    else if (p.WriteMode == DigitalWriteMode.TOGGLE)
                    {
                        // pin is initially set to  0 and toggles from there
                        TogglePin(p);
                    }
                }
            }
        }

        /// <summary>
        /// "Pulse Off" Activation mode
        /// pulls the pin to ground for a short amount of time
        /// </summary>
        private void PulseOff(DigitalWriteItem p)
        {
            arduino.DigitalWrite(p.PinId, p.SetPinState(0));
            Timer t = new Timer(PulseTime);
            t.Elapsed += new ElapsedEventHandler(
                (object sender, ElapsedEventArgs e) =>
                {
                    arduino.DigitalWrite(p.PinId, p.SetPinState(1));
                });

            t.AutoReset = false;
            t.Start();
        }

        /// <summary>
        /// "Pulse On" Activation mode
        /// pulls the pin to ground for a short amount of time
        /// </summary>
        private void PulseOn(DigitalWriteItem p)
        {
            arduino.DigitalWrite(p.PinId, p.SetPinState(1));
            Timer t = new Timer(PulseTime);
            t.Elapsed += new ElapsedEventHandler(
                (object sender, ElapsedEventArgs e) =>
                {
                    arduino.DigitalWrite(p.PinId, p.SetPinState(0));
                });
            t.AutoReset = false;
            t.Start();
        }

        /// <summary>
        /// "Toggle" Activation mode
        /// </summary>
        private void TogglePin(DigitalWriteItem p)
        {
            Logger.WriteLine("Pin " + p.PinId + " " + p.GetPinState());
            if (p.GetPinState() > 0)
            {
                arduino.DigitalWrite(p.PinId, p.SetPinState(0));
            }
            else
            {
                arduino.DigitalWrite(p.PinId, p.SetPinState(1));
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                return new ArduinoDigitalWriteConfig(this.writePins);
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            ArduinoDigitalWriteConfig config = configurationControl as ArduinoDigitalWriteConfig;
            writePins.Clear();

            arduinoPortName = config.ArduinoPortName;
            arduino = ArduinoFirmata.InstanceForPortname(arduinoPortName);

            if (arduino != null)
            {
                writePins = config.ActiveItems;


                foreach (DigitalWriteItem p in writePins)
                {
                    arduino.SetPinMode(p.PinId, PinMode.OUTPUT); 
                    if (p.WriteMode == DigitalWriteMode.HIGH)             
                    {
                        // pin will be set to HIGH
                    }
                    else if (p.WriteMode == DigitalWriteMode.LOW)         
                    {
                        // pin will be set to LOW
                    }
                    else if (p.WriteMode == DigitalWriteMode.PULSE_OFF) 
                    {
                        // pin will be set to OFF for a short period
                        arduino.DigitalWrite(p.PinId, p.SetPinState(1));
                    }
                    else if (p.WriteMode == DigitalWriteMode.PULSE_ON)   
                    {
                        // pin will be set to ON for a short period
                        arduino.DigitalWrite(p.PinId, p.SetPinState(0));
                    }
                    else if (p.WriteMode == DigitalWriteMode.TOGGLE)     
                    {
                        // pin is initially set to  0 and toggles from there
                        arduino.DigitalWrite(p.PinId, p.SetPinState(0));
                    }
                }
            }
        }

        public string GetConfigString()
        {
            string cs = arduinoPortName + " ";
            for (int i = 0; i < writePins.Count; i++)
            {
                if (i < writePins.Count - 1)
                    cs += writePins[i].PinName + ",";
                else
                    cs += writePins[i].PinName;
            }

            return cs;
        }
    }
}
