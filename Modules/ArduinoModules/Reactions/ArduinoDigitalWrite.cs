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
using MayhemCore.ModuleTypes;
using MayhemDefaultStyles.UserControls;
using System.Runtime.Serialization;
using ArduinoModules.Wpf;
using ArduinoModules.Wpf.Helpers;
using ArduinoModules.Firmata;

namespace ArduinoModules.Reactions
{
    [DataContract]
    [MayhemModule("Arduino Digital Write", "Writes logic values to a set of digital pins on the Arduino.")]
    public class ArduinoDigitalWrite : ReactionBase, IWpfConfigurable
    {

        private List<DigitalPinWriteItem> writePins = new List<DigitalPinWriteItem>();
        private string arduinoPortName = String.Empty;
        private ArduinoFirmata arduino = null; 

        public override void Perform()
        {
            //throw new NotImplementedException();
            if (arduino != null)
            {
                foreach (DigitalPinWriteItem p in writePins)
                {
                    if (p.WriteMode == DIGITAL_WRITE_MODE.HIGH)             // pin will be set to HIGH
                    {
                       // p.SetPinState(1);
                        arduino.DigitalWrite(p.GetPinID(), p.SetPinState(1));
                    }
                    else if (p.WriteMode == DIGITAL_WRITE_MODE.LOW)         // pin will be set to LOW
                    {
                        //p.SetPinState(0);
                        arduino.DigitalWrite(p.GetPinID(), p.SetPinState(0));
                    }
                    else if (p.WriteMode == DIGITAL_WRITE_MODE.PULSE_OFF) // pin will be set to OFF for a short period
                    {
                       
                    }
                    else if (p.WriteMode == DIGITAL_WRITE_MODE.PULSE_ON)   // pin will be set to ON for a short period
                    {
                        throw new NotImplementedException();
                    }
                    else if (p.WriteMode == DIGITAL_WRITE_MODE.TOGGLE)     // pin is initially set to  0 and toggles from there
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }

        public IWpfConfiguration ConfigurationControl
        {
            get 
            {
                return new ArduinoDigitalWriteConfig();
            }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
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
                        arduino.DigitalWrite(p.GetPinID(), p.GetPinState());
                    }
                    else if (p.WriteMode == DIGITAL_WRITE_MODE.PULSE_ON)   // pin will be set to ON for a short period
                    {

                        arduino.DigitalWrite(p.GetPinID(), p.SetPinState(1));
                    }
                    else if (p.WriteMode == DIGITAL_WRITE_MODE.TOGGLE)     // pin is initially set to  0 and toggles from there
                    {
                        arduino.DigitalWrite(p.GetPinID(), p.SetPinState(0));
                    }
                }
            }
        }

        public ArduinoDigitalWrite() { }
    }
}
