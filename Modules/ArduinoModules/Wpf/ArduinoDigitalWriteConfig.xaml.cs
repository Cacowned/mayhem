﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ArduinoModules.Firmata;
using ArduinoModules.Wpf.Helpers;
using MayhemCore;
using MayhemSerial;
using MayhemWpf.UserControls;

namespace ArduinoModules.Wpf
{
    /// <summary>
    /// Interaction logic for ArduinoDigitalWriteConfig.xaml
    /// </summary>
    public partial class ArduinoDigitalWriteConfig : WpfConfiguration, IArduinoEventListener
    {
        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.Instance;
        private Dictionary<string, string> deviceNamesIds = null;
        private ArduinoFirmata arduino = null;
        private ObservableCollection<DigitalWriteItem> pinItems = new ObservableCollection<DigitalWriteItem>();

        // pins already configured by the reaction 
        private List<DigitalWriteItem> reactionSetPins = null;

        public string ArduinoPortName
        {
            get
            {
                if (arduino != null)
                {
                    return arduino.PortName;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Returns list of current active items
        /// </summary>
        public List<DigitalWriteItem> ActiveItems
        {
            get
            {
                List<DigitalWriteItem> items = new List<DigitalWriteItem>();
                foreach (DigitalWriteItem p in pinItems)
                {
                    if (p.Active)
                        items.Add(p);
                }

                return items;
            }
        }

        public ArduinoDigitalWriteConfig(List<DigitalWriteItem> setPins)
        {
            reactionSetPins = setPins;
            InitializeComponent();
        }

        public override void OnLoad()
        {
            digitalPins.ItemsSource = pinItems;
            serial.UpdatePortList();
            deviceNamesIds = serial.GetArduinoPortNames();

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

            CanSave = true;
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            string portname = (string)deviceList.SelectedValue;

            bool updatePins = false;

            if (arduino != null)
            {
                // clear pin containers and unhook event ghandlers               
                pinItems.Clear();
                arduino.DeregisterListener(this);
            }

            // update pins ? 
            if (ArduinoFirmata.InstanceExists(portname))
                updatePins = true;

            arduino = ArduinoFirmata.InstanceForPortname(portname);
            arduino.RegisterListener(this);

            // update pins if the arduino already exists
            // this makes arduino call the OnPinAdded callbacks, which in turn
            // fill the gridViews with the detected pins 
            if (updatePins)
            {
                // reset analog ids to 0
                AnalogPinItem.ResetAnalogIDs();
                arduino.QueryPins();
            }

            connectButton.IsEnabled = false;
        }

        public override void OnClosing()
        {
            if (arduino != null)
            {
                arduino.DeregisterListener(this);
            }

            base.OnClosing();
        }

        private void digitalPins_AutoGeneratedColumns(object sender, EventArgs e)
        {
            (sender as DataGrid).Visibility = Visibility.Visible;
        }

        #region IArduinoEventListener

        public void Arduino_OnInitialized(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
        }

        public void Arduino_OnAnalogPinChanged(Pin p)
        {
            // throw new NotImplementedException();
        }

        public void Arduino_OnDigitalPinChanged(Pin p)
        {
            // throw new NotImplementedException();
        }

        public void Arduino_OnPinAdded(Pin p)
        {
            Logger.WriteLine("arduino_OnPinAdded: " + p.Id);
            if (p.Mode != PinMode.ANALOG &&
                p.Mode != PinMode.UNASSIGNED &&
                p.Mode != PinMode.SHIFT)
            {
                DigitalWriteItem pw = new DigitalWriteItem(false, p.Id, DigitalWriteMode.HIGH);

                // see if pin is already contained in set pins and take over the settings
                foreach (DigitalWriteItem setPin in reactionSetPins)
                {
                    if (setPin.PinId == p.Id)
                    {
                        // pin has already been configured
                        // use the existing DigitalPinWriteItem
                        pw = setPin;
                        Logger.WriteLine("Using already configured pin " + pw.PinId);
                    }
                }

                Dispatcher.Invoke(new Action(() => { pinItems.Add(pw); }), null);
            }
        }
        #endregion

        private void deviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;

            if (arduino != null && (string)box.SelectedValue != (string)arduino.PortName)
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
