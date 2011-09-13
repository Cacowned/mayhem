/*
 * ArduinoEvent.cs
 * 
 * An event that reacts to basic Arduino events such as pin state changes. 
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
        private const string TAG = "[ArduinoEvent] : ";
        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.instance;

        [DataMember]
        public string arduinoPortName = String.Empty;

        private ArduinoFirmata arduino = null; 

         //arduino.OnPinAdded += new Action<Pin>(arduino_OnPinAdded);
         //arduino.OnDigitalPinChanged += new Action<Pin>(arduino_OnDigitalPinChanged);
         //arduino.OnAnalogPinChanged += new Action<Pin>(arduino_OnAnalogPinChanged);
         

      
        private Action<Pin> OnDigitalPinChanged; // = new Action<Pin>(arduino_OnDigitalPinChanged);
        private Action<Pin> OnAnalogPinChanged; // = new Action<Pin>(arduino_OnAnalogPinChanged);

        private List<DigitalPinItem> monitorDigitalPins = new List<DigitalPinItem>();
        private List<AnalogPinItem> monitorAnalogPins = new List<AnalogPinItem>();


        public ArduinoEvent()
        {
            Initialize();
        }

        protected override void  Initialize()
        {
 	         base.Initialize() ;

             OnDigitalPinChanged = new Action<Pin>(arduino_OnDigitalPinChanged);
             OnAnalogPinChanged  = new Action<Pin>(arduino_OnAnalogPinChanged);
        }

        public IWpfConfiguration ConfigurationControl
        {
            get
            {
                Debug.WriteLine(TAG + "ConfigurationControl");

                // TODO
  
                ArduinoEventConfig config = new ArduinoEventConfig();           
                return config;
            }
        }



        public void OnSaved(IWpfConfiguration configurationControl)
        {
           
            ArduinoEventConfig config = configurationControl as ArduinoEventConfig;
            arduinoPortName = config.arduinoPortName; 
            arduino = ArduinoFirmata.InstanceForPortname(arduinoPortName);
            List<DigitalPinItem> digitalPinsMonitor = new List<DigitalPinItem>();
            List<AnalogPinItem> analogPinsMonitor = new List<AnalogPinItem>();

            // save references to pins to monitor
            // attach callbacks to arduino events
            if (arduino != null)
            {
                
                foreach (DigitalPinItem p in config.digital_pin_items)
                {
                    if (p.Selected)
                        digitalPinsMonitor.Add(p);
                }

                foreach (AnalogPinItem a in config.analog_pin_items)
                {
                    if (a.Selected)
                        analogPinsMonitor.Add(a);
                }

                monitorAnalogPins = analogPinsMonitor;
                monitorDigitalPins = digitalPinsMonitor; 

                arduino.OnAnalogPinChanged -= OnAnalogPinChanged;
                arduino.OnDigitalPinChanged -= OnDigitalPinChanged;
                arduino.OnAnalogPinChanged += OnAnalogPinChanged;
                arduino.OnDigitalPinChanged += OnDigitalPinChanged; 



            }



        }

        public void arduino_OnDigitalPinChanged(Pin p)
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
                            base.OnEventActivated();
                        }
                    }
                    else if (d.ChangeType == DIGITAL_PIN_CHANGE.HIGH)
                    {
                        if (d.GetDigitalPinState() > 0)
                        {
                            // fire
                            base.OnEventActivated();
                        }
                    }
                    else if (d.ChangeType == DIGITAL_PIN_CHANGE.LOW)
                    {
                        if (d.GetDigitalPinState() == 0)
                        {
                            // fire
                            base.OnEventActivated();
                        }
                    }
                    else if (d.ChangeType == DIGITAL_PIN_CHANGE.RISING)
                    {
                        if (p.value == 0 && d.GetDigitalPinState() > 0)
                        {
                            // fire
                            base.OnEventActivated();
                        }
                    }
                    p.value = d.GetDigitalPinState();
                }
            }
            
        }

        public void arduino_OnAnalogPinChanged(Pin p)
        {
            foreach (AnalogPinItem a in monitorAnalogPins)
            {
                if (a.GetPinID() == p.id)
                {
                    if (a.ChangeType == ANALOG_PIN_CHANGE.EQUALS)
                    {
                        if (a.SetValue == p.value)
                        {
                            base.OnEventActivated();
                        }
                    }
                    else if (a.ChangeType == ANALOG_PIN_CHANGE.GREATER)
                    {
                        if (a.SetValue <= p.value)
                        {
                            base.OnEventActivated();
                        }
                    }
                    else if (a.ChangeType == ANALOG_PIN_CHANGE.LOWER)
                    {
                        if (a.SetValue >= p.value)
                        {
                            base.OnEventActivated(); 
                        }
                    }
                    a.SetAnalogValue(p.value);
                }
            }
        }
    }
}
