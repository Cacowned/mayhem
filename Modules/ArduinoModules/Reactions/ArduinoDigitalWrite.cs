﻿using System;
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
        private List<DigitalPinWriteItem> writePins;
        [DataMember]
        private string arduinoPortName;

        private ArduinoFirmata arduino = null;

        // ms pulse time. 
        // TODO: evaluate if this may be required to
        // be set by the user
        private const int PulseTime = 20;

        protected override void OnLoadDefaults()
        {
            writePins = new List<DigitalPinWriteItem>();
            arduinoPortName = string.Empty;
        }

        protected override void OnAfterLoad()
        {
            if (arduinoPortName != string.Empty)
            {
                arduino = ArduinoFirmata.InstanceForPortname(arduinoPortName);
            }
        }

        public override void Perform()
        {
            if (arduino != null)
            {
                foreach (DigitalPinWriteItem p in writePins)
                {
                    if (p.WriteMode == DigitalWriteMode.High)
                    {
                        // pin will be set to HIGH
                        arduino.DigitalWrite(p.GetPinId(), p.SetPinState(1));
                    }
                    else if (p.WriteMode == DigitalWriteMode.Low)
                    {
                        // pin will be set to LOW
                        arduino.DigitalWrite(p.GetPinId(), p.SetPinState(0));
                    }
                    else if (p.WriteMode == DigitalWriteMode.PulseOff)
                    {
                        // pin will be set to OFF for a short period
                        PulseOff(p);
                    }
                    else if (p.WriteMode == DigitalWriteMode.PulseOn)
                    {
                        // pin will be set to ON for a short period
                        PulseOn(p);
                    }
                    else if (p.WriteMode == DigitalWriteMode.Toggle)
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
        private void PulseOff(DigitalPinWriteItem p)
        {
            arduino.DigitalWrite(p.GetPinId(), p.SetPinState(0));
            Timer t = new Timer(PulseTime);
            t.Elapsed += (sender, e) => arduino.DigitalWrite(p.GetPinId(), p.SetPinState(1));

            t.AutoReset = false;
            t.Start();
        }

        /// <summary>
        /// "Pulse On" Activation mode
        /// pulls the pin to ground for a short amount of time
        /// </summary>
        private void PulseOn(DigitalPinWriteItem p)
        {
            arduino.DigitalWrite(p.GetPinId(), p.SetPinState(1));
            Timer t = new Timer(PulseTime);
            t.Elapsed += (sender, e) => arduino.DigitalWrite(p.GetPinId(), p.SetPinState(0));

            t.AutoReset = false;
            t.Start();
        }

        /// <summary>
        /// "Toggle" Activation mode
        /// </summary>
        private void TogglePin(DigitalPinWriteItem p)
        {
            Logger.WriteLine("Pin " + p.GetPinId() + " " + p.GetPinState());
            if (p.GetPinState() > 0)
            {
                arduino.DigitalWrite(p.GetPinId(), p.SetPinState(0));
            }
            else
            {
                arduino.DigitalWrite(p.GetPinId(), p.SetPinState(1));
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

                foreach (DigitalPinWriteItem p in writePins)
                {
                    if (p.WriteMode == DigitalWriteMode.High)             
                    {
                        // pin will be set to HIGH
                    }
                    else if (p.WriteMode == DigitalWriteMode.Low)         
                    {
                        // pin will be set to LOW
                    }
                    else if (p.WriteMode == DigitalWriteMode.PulseOff) 
                    {
                        // pin will be set to OFF for a short period
                        arduino.DigitalWrite(p.GetPinId(), p.SetPinState(1));
                    }
                    else if (p.WriteMode == DigitalWriteMode.PulseOn)   
                    {
                        // pin will be set to ON for a short period
                        arduino.DigitalWrite(p.GetPinId(), p.SetPinState(0));
                    }
                    else if (p.WriteMode == DigitalWriteMode.Toggle)     
                    {
                        // pin is initially set to  0 and toggles from there
                        arduino.DigitalWrite(p.GetPinId(), p.SetPinState(0));
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
