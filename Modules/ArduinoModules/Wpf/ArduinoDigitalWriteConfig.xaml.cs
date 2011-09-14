﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MayhemDefaultStyles.UserControls;
using System.Collections.ObjectModel;
using ArduinoModules.Wpf.Helpers;
using ArduinoModules.Firmata;
using MayhemSerial;
using MayhemCore;

namespace ArduinoModules.Wpf
{
    /// <summary>
    /// Interaction logic for ArduinoDigitalWriteConfig.xaml
    /// </summary>
    public partial class ArduinoDigitalWriteConfig : IWpfConfiguration, IArduinoEventListener
    {

        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.instance;
        private Dictionary<string, string> deviceNamesIds = null;
        private ArduinoFirmata arduino = null;
        private ObservableCollection<DigitalPinWriteItem> pin_items = new ObservableCollection<DigitalPinWriteItem>();
        private List<DigitalPinWriteItem> reaction_set_pins = null;                     // pins already configured by the reaction 

        public string arduinoPortName
        {
            get
            {
                if (arduino != null)
                {
                    return arduino.portName;
                }
                return String.Empty;

            }
        }

        /// <summary>
        /// Returns list of current active items
        /// </summary>
        public List<DigitalPinWriteItem> active_items
        {
            get
            {
                List<DigitalPinWriteItem> items = new List<DigitalPinWriteItem>();
                foreach (DigitalPinWriteItem p in pin_items)
                {
                    if (p.Active)
                        items.Add(p);
                }
                return items; 
            }
        }
        

        public ArduinoDigitalWriteConfig(List<DigitalPinWriteItem> set_pins)
        {
            reaction_set_pins = set_pins;
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            digitalPins.ItemsSource = pin_items;
            serial.UpdatePortList();
            deviceNamesIds = serial.getArduinoPortNames();

            if (deviceNamesIds.Count > 0)
            {
                deviceList.ItemsSource = deviceNamesIds;
                deviceList.DisplayMemberPath = "Value";
                deviceList.SelectedValuePath = "Key";
                deviceList.SelectedIndex = 0;
            }

            // directly display the list if there is only one board attached
            if (deviceNamesIds.Count == 1)
            {
                connectButton_Click(this, null);
            }

       
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            string portname = (string)deviceList.SelectedValue;
            
            bool update_pins = false;

            if (arduino != null)
            {
                // clear pin containers and unhook event ghandlers               
                pin_items.Clear();
                arduino.DeregisterListener(this);

            }

            // update pins ? 
            if (ArduinoFirmata.InstanceExists(portname))
                update_pins = true;

            arduino = ArduinoFirmata.InstanceForPortname(portname);
            arduino.RegisterListener(this);

            // update pins if the arduino already exists
            // this makes arduino call the OnPinAdded callbacks, which in turn
            // fill the gridViews with the detected pins 
            if (update_pins)
            {
                // reset analog ids to 0
                AnalogPinItem.ResetAnalogIDs();
                arduino.QueryPins();
            }

            connectButton.IsEnabled = false;
        }

        public override void OnClosing()
        {
            arduino.DeregisterListener(this);
            base.OnClosing();
        }

        private void digitalPins_AutoGeneratedColumns(object sender, EventArgs e)
        {
            (sender as DataGrid).Visibility = Visibility.Visible;
        }

        #region IArduinoEventListener

        public void Arduino_OnInitialized(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void Arduino_OnAnalogPinChanged(Pin p)
        {
            //throw new NotImplementedException();
        }

        public void Arduino_OnDigitalPinChanged(Pin p)
        {
            //throw new NotImplementedException();
        }

        public void Arduino_OnPinAdded(Pin p)
        {
            //throw new NotImplementedException();
            Logger.WriteLine("arduino_OnPinAdded: " + p.id);
            if (p.mode != PIN_MODE.ANALOG &&
                p.mode != PIN_MODE.UNASSIGNED &&
                p.mode != PIN_MODE.SHIFT)
            {
                DigitalPinWriteItem pw = new DigitalPinWriteItem(false, p.id, DIGITAL_WRITE_MODE.HIGH);

                // see if pin is already contained in set pins and take over the settings
                foreach (DigitalPinWriteItem setPin in reaction_set_pins)
                {
                    if (setPin.GetPinID() == p.id)
                    {
                        // pin has already been configured
                        // use the existing DigitalPinWriteItem
                        pw = setPin;
                        Logger.WriteLine("Using already configured pin " + pw.GetPinID());
                    }
                }

                Dispatcher.Invoke(new Action(() => { pin_items.Add(pw); }), null);
            }
            

        }
        #endregion

        private void deviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;

            if (arduino != null && (string) box.SelectedValue != (string) arduino.portName)
            {
                connectButton.IsEnabled = true;
            }
            else
            {
                connectButton.IsEnabled = false;
            }
                
        }
    }
}
