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
        private string arduinoPortName;

        [DataMember]
        private List<DigitalPinItem> monitorDigitalPins;

        [DataMember]
        private List<AnalogPinItem> monitorAnalogPins;

        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.Instance;

        private ArduinoFirmata arduino = null;

        private Action<Pin> onDigitalPinChanged;
        private Action<Pin> onAnalogPinChanged;

        // minimum activation interval
        private const int ActivateMinDelay = 50;
        private DateTime lastActivated = DateTime.MinValue;

        protected override void OnLoadDefaults()
        {
            monitorDigitalPins = new List<DigitalPinItem>();
            monitorAnalogPins = new List<AnalogPinItem>();
            arduinoPortName = string.Empty;
        }

        protected override void OnAfterLoad()
        {
            if (arduinoPortName != string.Empty)
            {
                arduino = ArduinoFirmata.InstanceForPortname(arduinoPortName);
                foreach (DigitalPinItem pin in monitorDigitalPins)
                {
                    arduino.SetPinMode(pin.GetPinId(), PinMode.INPUT);
                }
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
            arduinoPortName = config.ArduinoPortName;
            arduino = ArduinoFirmata.InstanceForPortname(arduinoPortName);
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
                        arduino.SetPinMode(p.GetPinId(), PinMode.INPUT);
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
                Trigger();
                lastActivated = DateTime.Now;
            }
        }

        public void arduino_OnDigitalPinChanged(Pin p)
        {
            if (IsEnabled)
            {
                foreach (DigitalPinItem d in monitorDigitalPins)
                {
                    if (d.GetPinId() == p.Id)
                    {
                        if (d.ChangeType == DigitalPinChange.FALLING)
                        {
                            if (p.Value > 0 && d.GetDigitalPinState() == 0)
                            {
                                // fire
                                Activate();
                            }
                        }
                        else if (d.ChangeType == DigitalPinChange.HIGH)
                        {
                            if (p.Value > 0)
                            {
                                // fire
                                Activate();
                            }
                        }
                        else if (d.ChangeType == DigitalPinChange.LOW)
                        {
                            if (p.Value == 0)
                            {
                                // fire
                                Activate();
                            }
                        }
                        else if (d.ChangeType == DigitalPinChange.RISING)
                        {
                            if (p.Value == 0 && d.GetDigitalPinState() > 0)
                            {
                                // fire
                                Activate();
                            }
                        }

                        // p.value = d.GetDigitalPinState();
                        d.SetPinState(p.Value);
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
                    if (a.GetPinId() == p.Id)
                    {
                        if (a.ChangeType == AnalogPinChange.EQUALS)
                        {
                            if (a.SetValue == p.Value)
                            {
                                Activate();
                            }
                        }
                        else if (a.ChangeType == AnalogPinChange.GREATER)
                        {
                            if (a.SetValue <= p.Value)
                            {
                                Activate();
                            }
                        }
                        else if (a.ChangeType == AnalogPinChange.LOWER)
                        {
                            if (a.SetValue >= p.Value)
                            {
                                Activate();
                            }
                        }

                        a.SetAnalogValue(p.Value);
                    }
                }
            }
        }

        public string GetConfigString()
        {
            string cs = string.Empty;
            cs += arduinoPortName + " ";
            for (int i = 0; i < monitorDigitalPins.Count; i++)
            {
                if (i < monitorDigitalPins.Count - 1)
                    cs += monitorDigitalPins[i].PinName + ",";
                else
                    cs += monitorDigitalPins[i].PinName;
            }

            if (monitorAnalogPins.Count > 0)
                cs += ",";
            for (int i = 0; i < monitorAnalogPins.Count; i++)
            {
                if (i < monitorAnalogPins.Count - 1)
                    cs += monitorAnalogPins[i].PinName + ",";
                else
                    cs += monitorAnalogPins[i].PinName;
            }

            return cs;
        }
    }
}
