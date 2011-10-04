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
using System.Runtime.Serialization;
using ArduinoModules.Firmata;
using ArduinoModules.Wpf;
using ArduinoModules.Wpf.Helpers;
using MayhemCore;
using MayhemSerial;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace ArduinoModules.Events
{
    [DataContract]
    [MayhemModule("Arduino Event", "**Testing** Detects Pin Changes in Arduino")]
    public class ArduinoEvent : EventBase, IWpfConfigurable
    {
        [DataMember]
        public string ArduinoPortName;

        [DataMember]
        private List<DigitalPinItem> monitorDigitalPins;

        [DataMember]
        private List<AnalogPinItem> monitorAnalogPins;

        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.Instance;

        private ArduinoFirmata arduino = null;

        private Action<Pin> onDigitalPinChanged;
        private Action<Pin> onAnalogPinChanged;

        private const int ActivateMinDelay = 50;  //minimum activation interval
        DateTime lastActivated = DateTime.MinValue;

        protected override void OnLoadDefaults()
        {
            monitorDigitalPins = new List<DigitalPinItem>();
            monitorAnalogPins = new List<AnalogPinItem>();
            ArduinoPortName = String.Empty;
        }

        protected override void OnAfterLoad()
        {
            if (ArduinoPortName != String.Empty)
            {
                arduino = ArduinoFirmata.InstanceForPortname(ArduinoPortName);
            }

            onDigitalPinChanged = new Action<Pin>(arduino_OnDigitalPinChanged);
            onAnalogPinChanged = new Action<Pin>(arduino_OnAnalogPinChanged);
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


        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (onAnalogPinChanged != null && onDigitalPinChanged != null && arduino != null)
            {
                arduino.OnAnalogPinChanged -= onAnalogPinChanged;
                arduino.OnDigitalPinChanged -= onDigitalPinChanged;

                arduino.OnAnalogPinChanged += onAnalogPinChanged;
                arduino.OnDigitalPinChanged += onDigitalPinChanged;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (arduino != null)
            {
                arduino.OnAnalogPinChanged -= onAnalogPinChanged;
                arduino.OnDigitalPinChanged -= onDigitalPinChanged;
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {

            ArduinoEventConfig config = configurationControl as ArduinoEventConfig;
            ArduinoPortName = config.ArduinoPortName;
            arduino = ArduinoFirmata.InstanceForPortname(ArduinoPortName);
            List<DigitalPinItem> digitalPinsMonitor = new List<DigitalPinItem>();
            List<AnalogPinItem> analogPinsMonitor = new List<AnalogPinItem>();

            bool enabled = this.IsEnabled;

            // save references to pins to monitor
            // attach callbacks to arduino events
            if (arduino != null)
            {
                arduino.OnAnalogPinChanged -= onAnalogPinChanged;
                arduino.OnDigitalPinChanged -= onDigitalPinChanged;

                foreach (DigitalPinItem p in config.DigitalPinItems)
                {
                    if (p.Selected)
                    {
                        // TODO: arduino.FlagPin(p)
                        digitalPinsMonitor.Add(p);
                    }
                }

                foreach (AnalogPinItem a in config.AnalogPinItems)
                {
                    // TODO: arduino.FlagPin(p)
                    if (a.Selected)
                        analogPinsMonitor.Add(a);
                }

                monitorAnalogPins = analogPinsMonitor;
                monitorDigitalPins = digitalPinsMonitor;

                if (enabled)
                {
                    arduino.OnAnalogPinChanged += onAnalogPinChanged;
                    arduino.OnDigitalPinChanged += onDigitalPinChanged;
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

            if (ts.TotalMilliseconds >= ActivateMinDelay)
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
                    if (d.GetPinId() == p.id)
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
                    if (a.GetPinId() == p.id)
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
            // throw new Exception("Sven: fill this in");

            string cs = string.Empty;
            cs += ArduinoPortName+" ";
            for (int i = 0; i < monitorDigitalPins.Count; i++)
            {
                if (i < monitorDigitalPins.Count - 1)
                    cs += monitorDigitalPins[i].PinName + ",";                
                else
                    cs += monitorDigitalPins[i].PinName ;
            }
            if (monitorAnalogPins.Count > 0)
                cs += ",";
            for (int i = 0; i < monitorAnalogPins.Count; i++)
            {
                if (i < monitorAnalogPins.Count - 1)
                    cs += monitorAnalogPins[i].PinName + ",";
                else
                    cs += monitorAnalogPins[i].PinName ;
            }

            return cs;
        }
    }
}
