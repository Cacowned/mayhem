/*
 * ArduinoEvent.cs
 * 
 * An event that triggers to basic Arduino pin state changes. 
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
using System.Diagnostics;
using ArduinoModules.Wpf;
using System.Runtime.Serialization;
using MayhemSerial;
using ArduinoModules.Firmata;

namespace ArduinoModules.Events
{
    [DataContract]
    [MayhemModule("Arduino Event", "**Testing** Detects Pin Changes in Arduino")]
    public class ArduinoEvent : EventBase, IWpfConfigurable
    {
        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.instance;

        private ArduinoFirmata arduino = null;

        private Action<Pin> OnDigitalPinChanged; // = new Action<Pin>(arduino_OnDigitalPinChanged);
        private Action<Pin> OnAnalogPinChanged; // = new Action<Pin>(arduino_OnAnalogPinChanged);

        private const int ACTIVATE_MIN_DELAY = 50;  //minimum activation interval
        DateTime lastActivated = DateTime.MinValue;

        [DataMember]
        public string arduinoPortName = String.Empty;

        [DataMember]
        private List<DigitalPinItem> monitorDigitalPins = new List<DigitalPinItem>();

        [DataMember]
        private List<AnalogPinItem> monitorAnalogPins = new List<AnalogPinItem>();

        protected override void Initialize()
        {
            if (arduinoPortName != String.Empty)
            {
                arduino = ArduinoFirmata.InstanceForPortname(arduinoPortName);
            }

            OnDigitalPinChanged = new Action<Pin>(arduino_OnDigitalPinChanged);
            OnAnalogPinChanged = new Action<Pin>(arduino_OnAnalogPinChanged);
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                Logger.WriteLine("ConfigurationControl");

                // TODO

                ArduinoEventConfig config = new ArduinoEventConfig(monitorDigitalPins, monitorAnalogPins);
                return config;
            }
        }


        protected override bool OnEnable()
        {
            if (OnAnalogPinChanged != null && OnDigitalPinChanged != null && arduino != null)
            {

                arduino.OnAnalogPinChanged -= OnAnalogPinChanged;
                arduino.OnDigitalPinChanged -= OnDigitalPinChanged;

                arduino.OnAnalogPinChanged += OnAnalogPinChanged;
                arduino.OnDigitalPinChanged += OnDigitalPinChanged;
            }

            return true;
        }

        protected override void OnDisable()
        {
            if (arduino != null)
            {
                arduino.OnAnalogPinChanged -= OnAnalogPinChanged;
                arduino.OnDigitalPinChanged -= OnDigitalPinChanged;
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {

            ArduinoEventConfig config = configurationControl as ArduinoEventConfig;
            arduinoPortName = config.arduinoPortName;
            arduino = ArduinoFirmata.InstanceForPortname(arduinoPortName);
            List<DigitalPinItem> digitalPinsMonitor = new List<DigitalPinItem>();
            List<AnalogPinItem> analogPinsMonitor = new List<AnalogPinItem>();

            bool enabled = this.IsEnabled;

            // save references to pins to monitor
            // attach callbacks to arduino events
            if (arduino != null)
            {
                arduino.OnAnalogPinChanged -= OnAnalogPinChanged;
                arduino.OnDigitalPinChanged -= OnDigitalPinChanged;

                foreach (DigitalPinItem p in config.digital_pin_items)
                {
                    if (p.Selected)
                    {
                        // TODO: arduino.FlagPin(p)
                        digitalPinsMonitor.Add(p);
                    }
                }

                foreach (AnalogPinItem a in config.analog_pin_items)
                {
                    // TODO: arduino.FlagPin(p)
                    if (a.Selected)
                        analogPinsMonitor.Add(a);
                }

                monitorAnalogPins = analogPinsMonitor;
                monitorDigitalPins = digitalPinsMonitor;

                if (enabled)
                {
                    arduino.OnAnalogPinChanged += OnAnalogPinChanged;
                    arduino.OnDigitalPinChanged += OnDigitalPinChanged;
                }
            }
        }

        /// <summary>
        /// Limit activation rate to 10ms
        /// </summary>
        private void Activate()
        {
            DateTime now = DateTime.Now;

            TimeSpan ts = now - lastActivated;

            if (ts.TotalMilliseconds >= ACTIVATE_MIN_DELAY)
            {
                // call activate on base
                base.Trigger();
                lastActivated = DateTime.Now;
            }
        }

        public void arduino_OnDigitalPinChanged(Pin p)
        {
            if (IsEnabled)
            {
                foreach (DigitalPinItem d in monitorDigitalPins)
                {
                    if (d.GetPinID() == p.id)
                    {
                        if (d.ChangeType == DIGITAL_PIN_CHANGE.FALLING)
                        {
                            if (p.value > 0 && d.GetDigitalPinState() == 0)
                            {
                                // fire
                                Activate();
                            }
                        }
                        else if (d.ChangeType == DIGITAL_PIN_CHANGE.HIGH)
                        {
                            if (p.value > 0)
                            {
                                // fire
                                Activate();
                            }
                        }
                        else if (d.ChangeType == DIGITAL_PIN_CHANGE.LOW)
                        {
                            if (p.value == 0)
                            {
                                // fire
                                Activate();
                            }
                        }
                        else if (d.ChangeType == DIGITAL_PIN_CHANGE.RISING)
                        {
                            if (p.value == 0 && d.GetDigitalPinState() > 0)
                            {
                                // fire
                                Activate();
                            }
                        }
                        // p.value = d.GetDigitalPinState();
                        d.SetPinState(p.value);
                    }
                }
            }
        }


        public void arduino_OnAnalogPinChanged(Pin p)
        {
            if (this.IsEnabled)
            {
                foreach (AnalogPinItem a in monitorAnalogPins)
                {
                    if (a.GetPinID() == p.id)
                    {
                        if (a.ChangeType == ANALOG_PIN_CHANGE.EQUALS)
                        {
                            if (a.SetValue == p.value)
                            {
                                Activate();
                            }
                        }
                        else if (a.ChangeType == ANALOG_PIN_CHANGE.GREATER)
                        {
                            if (a.SetValue <= p.value)
                            {
                                Activate();
                            }
                        }
                        else if (a.ChangeType == ANALOG_PIN_CHANGE.LOWER)
                        {
                            if (a.SetValue >= p.value)
                            {
                                Activate();
                            }
                        }
                        a.SetAnalogValue(p.value);
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
