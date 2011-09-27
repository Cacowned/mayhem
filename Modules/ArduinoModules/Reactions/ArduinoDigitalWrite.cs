/*
 * ArduinoDigitalWrite.cs
 * 
 * Writes logic values to a set of digital pins on the Arduino.
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using System.Runtime.Serialization;
using ArduinoModules.Wpf;
using ArduinoModules.Wpf.Helpers;
using ArduinoModules.Firmata;
using System.Timers;

namespace ArduinoModules.Reactions
{
    [DataContract]
    [MayhemModule("Arduino Digital Write", "Writes logic values to a set of digital pins on the Arduino.")]
    public class ArduinoDigitalWrite : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private List<DigitalPinWriteItem> writePins = new List<DigitalPinWriteItem>();
        [DataMember]
        private string arduinoPortName = String.Empty;
        private ArduinoFirmata arduino = null;

        private const int pulse_time = 20;                          // ms pulse time. 
                                                                    // TODO: evaluate if this may be required to
                                                                    // be set by the user
        protected override void Initialize()
        {
            if (arduinoPortName != String.Empty)
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
                    if (p.WriteMode == DIGITAL_WRITE_MODE.HIGH)             // pin will be set to HIGH
                    {                      
                        arduino.DigitalWrite(p.GetPinID(), p.SetPinState(1));
                    }
                    else if (p.WriteMode == DIGITAL_WRITE_MODE.LOW)         // pin will be set to LOW
                    {                      
                        arduino.DigitalWrite(p.GetPinID(), p.SetPinState(0));
                    }
                    else if (p.WriteMode == DIGITAL_WRITE_MODE.PULSE_OFF) // pin will be set to OFF for a short period
                    {
                        PulseOff(p);
                    }
                    else if (p.WriteMode == DIGITAL_WRITE_MODE.PULSE_ON)   // pin will be set to ON for a short period
                    {
                        PulseOn(p);
                    }
                    else if (p.WriteMode == DIGITAL_WRITE_MODE.TOGGLE)     // pin is initially set to  0 and toggles from there
                    {
                        //throw new NotImplementedException();
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
            arduino.DigitalWrite(p.GetPinID(), p.SetPinState(0));
            Timer t = new Timer(pulse_time);
            t.Elapsed += new ElapsedEventHandler(
                (object sender, ElapsedEventArgs e) => { arduino.DigitalWrite(p.GetPinID(), p.SetPinState(1)); }
                );
            t.AutoReset = false;
            t.Start();
        }

        /// <summary>
        /// "Pulse On" Activation mode
        /// pulls the pin to ground for a short amount of time
        /// </summary>
        private void PulseOn(DigitalPinWriteItem p)
        {
            arduino.DigitalWrite(p.GetPinID(), p.SetPinState(1));
            Timer t = new Timer(pulse_time);
            t.Elapsed += new ElapsedEventHandler(
                (object sender, ElapsedEventArgs e) => { arduino.DigitalWrite(p.GetPinID(), p.SetPinState(0)); }
                );
            t.AutoReset = false;
            t.Start();
        }

        /// <summary>
        /// "Toggle" Activation mode
        /// </summary>
        private void TogglePin(DigitalPinWriteItem p)
        {                  
            Logger.WriteLine("Pin " + p.GetPinID() + " "+ p.GetPinState());
            if (p.GetPinState() > 0)
            {
                arduino.DigitalWrite(p.GetPinID(), p.SetPinState(0));
            }
            else
            {
                arduino.DigitalWrite(p.GetPinID(), p.SetPinState(1));
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
           // throw new NotImplementedException();
            ArduinoDigitalWriteConfig config = configurationControl as ArduinoDigitalWriteConfig;
            writePins.Clear();

            arduinoPortName = config.arduinoPortName;
            arduino = ArduinoFirmata.InstanceForPortname(arduinoPortName);

            if (arduino != null)
            {
                writePins = config.active_items;

                foreach (DigitalPinWriteItem p in writePins)
                {
                    if (p.WriteMode == DIGITAL_WRITE_MODE.HIGH)             // pin will be set to HIGH
                    {
                        // TODO
                    }
                    else if (p.WriteMode == DIGITAL_WRITE_MODE.LOW)         // pin will be set to LOW
                    {
                        // TODO
                    }
                    else if (p.WriteMode == DIGITAL_WRITE_MODE.PULSE_OFF) // pin will be set to OFF for a short period
                    {
                        arduino.DigitalWrite(p.GetPinID(), p.SetPinState(1));
                    }
                    else if (p.WriteMode == DIGITAL_WRITE_MODE.PULSE_ON)   // pin will be set to ON for a short period
                    {

                        arduino.DigitalWrite(p.GetPinID(), p.SetPinState(0));
                    }
                    else if (p.WriteMode == DIGITAL_WRITE_MODE.TOGGLE)     // pin is initially set to  0 and toggles from there
                    {
                        arduino.DigitalWrite(p.GetPinID(), p.SetPinState(0));
                    }
                }
            }
        }

        public string GetConfigString()
        {
            ///TODO: Sven: Put the right thing here
            throw new Exception("Sven: fill this in");
        }
    }
}
