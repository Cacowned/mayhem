/*
 * 
 * ArduinoEventConfig.xaml.cs
 * 
 * 
 * Code-Behind for ArduinoEventConfig
 * 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 * 
 */ 


using System;
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
using MayhemSerial;
using ArduinoModules.Firmata;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading; 

namespace ArduinoModules.Wpf
{

    /// <summary>
    /// Interaction logic for ArduinoEventConfig.xaml
    /// </summary>
    public partial class ArduinoEventConfig : IWpfConfiguration
    {
        public static string TAG = "[ArduinoEventConfig] :";
        

        /// <summary>
        /// Items for Ditial pins ItemsControl 
        /// </summary>
        public class DigitalPinItem
        {
            // selected
            private bool isChecked = false;
            public bool Selected { get { return isChecked; } set { isChecked = value; } }

            // friendly name
            private string pinName_;
            public string PinName
            {
                get { return pinName_; }
            }

            // change type
            private DIGITAL_PIN_CHANGE monitor_pin_change_ { get; set; }
            public DIGITAL_PIN_CHANGE ChangeType { get { return monitor_pin_change_; } set { monitor_pin_change_ = value; } }

            private int firmata_pin_id_ = 0;
            //public int pin_id { get { return pin_id_; } }

            // state
            private bool digitalPinState = false;

           
            public string PinState
            {
                get  {
                    if (digitalPinState)
                        return "HIGH";
                    else
                        return "LOW";
                    }
            }

            /// <summary>
            /// Set the digital pin state. Explicitly implemented setter, to be able to provide a string
            /// representation to a datagrid.
            /// </summary>
            /// <param name="state"></param>
            public void SetPinState(bool state)
            {
                digitalPinState = state; 
            }
           
      
            public DigitalPinItem(bool check, int id, DIGITAL_PIN_CHANGE change)
            {
                isChecked = check;
                pinName_ = "D" + id;
                monitor_pin_change_ = change;
                firmata_pin_id_ = id; 
            }

            
        }

        /// <summary>
        /// Items for Ditial pins ItemsControl 
        /// </summary>
        public class AnalogPinItem 
        {
            private static int analog_pin_id = 0; 

            // selected 
            private bool isChecked;
            public bool Selected { get { return isChecked; } set { isChecked = value; } }


            // friendly Name
            private string pinName_;
            public string PinName
            {
                get { return pinName_; }
            }

            // pin change type
            private ANALOG_PIN_CHANGE monitor_pin_change { get; set; }
            public ANALOG_PIN_CHANGE ChangeType { get { return monitor_pin_change; } set { monitor_pin_change = value; } }

            private int firmata_pin_id_ = 0;
            //public int pin_id { get { return pin_id_; } }

            private int setValue = 0;
        
            // change threshold value set by user
            public int SetValue { 
                get
                {
                  return setValue;  
                } 
                
                // constrain the settable value to a 16 bit range
                set
                {
                   if (value >= 0 && value <=1024)
                   {
                       setValue = value;
                   }
                   else
                   {
                       if (value <= 0)
                           setValue = 0;
                       else if (value >= 1024)
                           setValue = 1024; 
                   }
                }
            
            }


         

            // analog value
            private int aValue = 0;
            public int AnalogValue { get { return aValue; } }

            /// <summary>
            /// Sets the analog value for display
            /// Not implemented as setter for AnalogValue, as we want this to be read-only in the
            /// data grid.
            /// </summary>
            /// <param name="value"></param>
            public void SetAnalogValue (int value)
            {
                aValue = (int) value;
            }

            public AnalogPinItem(bool check, int id, ANALOG_PIN_CHANGE change)
            {
                isChecked = check;
                pinName_ = "A" + analog_pin_id++;
                monitor_pin_change = change;
                firmata_pin_id_ = id;
                setValue = 512; 
            }
        }


        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.instance;
        private int itemSelected = -1;
        private Dictionary<string, string>  deviceNamesIds = null;
        private ArduinoFirmata arduino = null;

        // collections driving the itemPanels
        public ObservableCollection<DigitalPinItem> digital_pin_items = new ObservableCollection<DigitalPinItem>();
        public ObservableCollection<AnalogPinItem> analog_pin_items = new ObservableCollection<AnalogPinItem>();

        private BackgroundWorker bg_pinUpdate = new BackgroundWorker();

        public ArduinoEventConfig()
        {
            InitializeComponent();
            digitalPins.ItemsSource = digital_pin_items;
            analogPins.ItemsSource = analog_pin_items;
            serial.UpdatePortList();
            deviceNamesIds = serial.getArduinoPortNames();
  
            if (deviceNamesIds.Count > 0)
            {
                deviceList.ItemsSource = deviceNamesIds;
                deviceList.DisplayMemberPath = "Value";
                deviceList.SelectedValuePath = "Key";
                deviceList.SelectedIndex = 0; 
            }

            //////// DEBUG
            //DigitalPinItem pItem = new DigitalPinItem(false, -3, DIGITAL_PIN_CHANGE.LOW);
            //digital_pin_items.Add(pItem);
            //pItem = new DigitalPinItem(false, -2, DIGITAL_PIN_CHANGE.LOW);
            //digital_pin_items.Add(pItem);
            //pItem = new DigitalPinItem(false, -1, DIGITAL_PIN_CHANGE.LOW);
            //digital_pin_items.Add(pItem);

            //AnalogPinItem dummy = new AnalogPinItem(false, -3, ANALOG_PIN_CHANGE.EQUALS);
            //analog_pin_items.Add(dummy);
            //dummy = new AnalogPinItem(false, -2, ANALOG_PIN_CHANGE.EQUALS);
            //analog_pin_items.Add(dummy); 

            // background workers

            //bg_analogPinUpdate.DoWork += new DoWorkEventHandler(this.updateAnalogPins);

            bg_pinUpdate.DoWork += new DoWorkEventHandler((object o, DoWorkEventArgs e) => 
            
            {
                Dispatcher.Invoke(new Action(() => {digitalPins.Items.Refresh();}),  DispatcherPriority.Background);
                Dispatcher.Invoke(new Action(() => { analogPins.Items.Refresh(); }), DispatcherPriority.Background);
            });
            
        }

       

        private void IWpfConfiguration_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                if (itemSelected < 0)
                {
                    serial.UpdatePortList();
                   // deviceList.ItemsSource = serial.serialPortNames;
                    deviceList.SelectedIndex = 0;
                }

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string portname = (string)deviceList.SelectedValue;

            if (arduino == null)
                arduino = new ArduinoFirmata(portname);

           
            arduino.OnPinAdded += new Action<Pin>(arduino_OnPinAdded);
            arduino.OnDigitalPinChanged += new Action<Pin>(arduino_OnDigitalPinChanged);
            arduino.OnAnalogPinChanged += new Action<Pin>(arduino_OnAnalogPinChanged);
            arduino.OnInitialized += new EventHandler(arduino_OnInitialized);
               
           

            
    
        }

        void arduino_OnInitialized(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Dispatcher.Invoke(new Action(() => { 
                connectButton.Content = "Connected";
                connectButton.IsEnabled = false;
            }), null);
            
        }

        #region Arduino Event Handlers

        void arduino_OnAnalogPinChanged(Pin p)
        {
            //throw new NotImplementedException();
            //Debug.WriteLine(TAG+ "arduino_OnAnalogPinChanged: " + p.analog_channel + " v:"+p.value );
            if (p.analog_channel < analog_pin_items.Count)
            {
               
                // refresh data grid items
                analog_pin_items[p.analog_channel].SetAnalogValue(p.value);
                if (!bg_pinUpdate.IsBusy)
                    bg_pinUpdate.RunWorkerAsync();
                //    bg_analogPinUpdate.RunWorkerAsync();
                //Dispatcher.Invoke(new Action(() => { analogPins.Items.Refresh(); }), DispatcherPriority.ApplicationIdle);

            }


        }

        void arduino_OnDigitalPinChanged(Pin p)
        {
            //throw new NotImplementedException();
            Debug.WriteLine(TAG + "arduino_OnDigitalPinChanged " + p.id + " " + p.value);

            if (p.id < digital_pin_items.Count)
            {
               bool state =  (p.value > 0) ? true : false;
               digital_pin_items[p.id].SetPinState(state);
               // run the state update in the background
               if (!bg_pinUpdate.IsBusy)
                   bg_pinUpdate.RunWorkerAsync();
               //Dispatcher.Invoke(new Action(() => {digitalPins.Items.Refresh();}),DispatcherPriority.ApplicationIdle);
            }


        }

        private void arduino_OnPinAdded(Pin p)
        {
            //throw new NotImplementedException();
            Debug.WriteLine("arduino_OnPinAdded: " + p.analog_channel );

            if (p.mode != PIN_MODE.ANALOG && 
                p.mode != PIN_MODE.UNASSIGNED && 
                p.mode != PIN_MODE.SHIFT)
            {
                DigitalPinItem pItem = new DigitalPinItem(false, p.id, DIGITAL_PIN_CHANGE.LOW);
                Dispatcher.Invoke(new Action(() => { digital_pin_items.Add(pItem); }), null); 
                // set digital pin mode to input
                if (p.mode != PIN_MODE.INPUT)
                {
                    arduino.SetPinMode(p, PIN_MODE.INPUT);
                }
            }
            else if (p.mode == PIN_MODE.ANALOG)
            {
                AnalogPinItem aItem = new AnalogPinItem(false, p.id, ANALOG_PIN_CHANGE.EQUALS);
                Dispatcher.Invoke(new Action(() => { analog_pin_items.Add(aItem);}), null);
            }



        }

        #endregion


        #region Column Auto-Generation
        
        private void digitalPins_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
       

        }

        private void digitalPins_AutoGeneratedColumns(object sender, EventArgs e)
        {
            DataGrid d = sender as DataGrid;
            d.Visibility = Visibility.Visible;
        }

        private void analogPins_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

        }

        private void analogPins_AutoGeneratedColumns(object sender, EventArgs e)
        {
            DataGrid d = sender as DataGrid;
            d.Visibility = Visibility.Visible;
        }

        #endregion


   
    }
}
