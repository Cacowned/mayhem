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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ArduinoModules.Firmata;
using MayhemCore;
using MayhemSerial;
using MayhemWpf.UserControls;
using ArduinoModules.Wpf.Helpers; 

namespace ArduinoModules.Wpf
{

    /// <summary>
    /// Interaction logic for ArduinoEventConfig.xaml
    /// </summary>
    public partial class ArduinoEventConfig : WpfConfiguration, IArduinoEventListener
    {     
        
        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.Instance;
        private int itemSelected = -1;
        private Dictionary<string, string>  deviceNamesIds = null;
        private ArduinoFirmata arduino = null;

        // collections driving the itemPanels
        public ObservableCollection<DigitalPinItem> DigitalPinItems = new ObservableCollection<DigitalPinItem>();
        public ObservableCollection<AnalogPinItem> AnalogPinItems = new ObservableCollection<AnalogPinItem>();


        Timer t = new Timer(1000);


        private List<DigitalPinItem> preset_digital_pins = null;
        private List<AnalogPinItem> preset_analog_pins = null; 


        private BackgroundWorker bg_pinUpdate = new BackgroundWorker();

        public string ArduinoPortName
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

        public ArduinoEventConfig(List<DigitalPinItem> reactionDigitalPins, List<AnalogPinItem> reactionAnalogPins )
        {
            preset_digital_pins = reactionDigitalPins;
            preset_analog_pins = reactionAnalogPins;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            digitalPins.ItemsSource = DigitalPinItems;
            analogPins.ItemsSource = AnalogPinItems;
            serial.UpdatePortList();
            deviceNamesIds = serial.GetArduinoPortNames();

            if (deviceNamesIds.Count > 0)
            {
                deviceList.ItemsSource = deviceNamesIds;
                deviceList.DisplayMemberPath = "Value";
                deviceList.SelectedValuePath = "Key";
                deviceList.SelectedIndex = 0;
            }

            // connect to board if there is only one arduino present
            if (deviceNamesIds.Count == 1)
            {
                Button_Click(this, null);
            }

            bg_pinUpdate.DoWork += new DoWorkEventHandler((object o, DoWorkEventArgs e) =>
            {
                Dispatcher.BeginInvoke(new Action(() => { digitalPins.Items.Refresh(); }), DispatcherPriority.Render);
                Dispatcher.BeginInvoke(new Action(() => { analogPins.Items.Refresh(); }), DispatcherPriority.Render);
            });

            bg_pinUpdate.WorkerSupportsCancellation = true;

            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            t.Enabled = true;

            CanSave = true; 
        }

        private void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            //throw new NotImplementedException();
            if (!bg_pinUpdate.IsBusy)
                bg_pinUpdate.RunWorkerAsync();
        }

        #region WPF Events
       

        /// <summary>
        /// Executes intial connection with the Arduino Board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {    
            string portname = (string)deviceList.SelectedValue;
            bool updatePins = false; 

            if (arduino != null)
            {
                // clear pin containers and unhook event ghandlers
                AnalogPinItems.Clear();
                DigitalPinItems.Clear();
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
                Arduino_OnInitialized(this, null);
            }

        }

        private void deviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;

            if (arduino != null && (string)box.SelectedValue != (string)arduino.portName)
            {
                connectButton.IsEnabled = true;
            }
            else
            {
                connectButton.IsEnabled = false;
            }
        }
        #endregion

        #region IWpfConfigurable overrides
        public override void OnClosing()
        {
            if (arduino != null)
            {
                arduino.DeregisterListener(this);
            }
            t.Enabled = false;
            bg_pinUpdate.CancelAsync();
            bg_pinUpdate.Dispose();
        }
     

        public override string Title
        {
            get
            {
                return "Arduino Event";
            }
        }
        #endregion


        #region IArduinoEventListener

        public void Arduino_OnInitialized(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Dispatcher.Invoke(new Action(() =>
            {
                connectButton.Content = "Connected";
                connectButton.IsEnabled = false;
            }), null);

        }

        public void Arduino_OnAnalogPinChanged(Pin p)
        {
            //throw new NotImplementedException();
            //Debug.WriteLine(TAG+ "arduino_OnAnalogPinChanged: " + p.analog_channel + " v:"+p.value );
            if (p.analog_channel < AnalogPinItems.Count)
            {             
                // refresh data grid items
                AnalogPinItems[p.analog_channel].SetAnalogValue(p.value);               
            }
        }

        public void Arduino_OnDigitalPinChanged(Pin p)
        {
            //throw new NotImplementedException();
            Logger.WriteLine( "arduino_OnDigitalPinChanged " + p.id + " " + p.value);

            if (p.id < DigitalPinItems.Count)
            {
               
                foreach (DigitalPinItem pin in DigitalPinItems)
                {
                    if (pin.GetPinId() == p.id)
                        pin.SetPinState(p.value);
                }
                    
            }
        }

        public void Arduino_OnPinAdded(Pin p)
        {
            //throw new NotImplementedException();
            Logger.WriteLine("arduino_OnPinAdded: " + p.id );

            if (p.mode != PIN_MODE.ANALOG && 
                p.mode != PIN_MODE.UNASSIGNED && 
                p.mode != PIN_MODE.SHIFT)
            {
                DigitalPinItem pItem = new DigitalPinItem(false, p.id, DIGITAL_PIN_CHANGE.LOW);
                foreach (DigitalPinItem setPin in preset_digital_pins)
                {
                    if (setPin.GetPinId() == p.id)
                    {
                        pItem = setPin;
                        Logger.WriteLine("Using already configured pin " + setPin.GetPinId());
                    }
                }
                Dispatcher.Invoke(new Action(() => { DigitalPinItems.Add(pItem); }), null); 
                // set digital pin mode to input
                if (p.mode != PIN_MODE.INPUT && p.flagged == false)
                {
                    arduino.SetPinMode(p, PIN_MODE.INPUT);
                }
            }
            else if (p.mode == PIN_MODE.ANALOG)
            {
               
                AnalogPinItem aItem = new AnalogPinItem(false, p.id, ANALOG_PIN_CHANGE.EQUALS);
                foreach (AnalogPinItem setPin in preset_analog_pins)
                {
                    if (setPin.GetPinId() == p.id)
                    {
                        aItem = setPin;
                        Logger.WriteLine("Using already configured pin " + setPin.GetPinId());
                    }
                }
                Dispatcher.Invoke(new Action(() => { AnalogPinItems.Add(aItem);}), null);
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

        

        #region GridView Responsiveness while updating 
       
        private void digitalPins_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            t.Enabled = false;
            while (bg_pinUpdate.IsBusy) ;

            (sender as DataGrid).RowEditEnding -= digitalPins_RowEditEnding;
            (sender as DataGrid).CommitEdit();
            // (sender as DataGrid).Items.Refresh();
            (sender as DataGrid).RowEditEnding += digitalPins_RowEditEnding;

            bg_pinUpdate.RunWorkerAsync();
            t.Enabled = true;

        }

        private void digitalPins_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            bg_pinUpdate.CancelAsync();
            t.Enabled = false;
            while (bg_pinUpdate.IsBusy) ;
        }

        private void analogPins_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            bg_pinUpdate.CancelAsync();
            t.Enabled = false;
            while (bg_pinUpdate.IsBusy) ;

            (sender as DataGrid).RowEditEnding -= analogPins_RowEditEnding;
            (sender as DataGrid).CommitEdit();        
            (sender as DataGrid).RowEditEnding += analogPins_RowEditEnding;

            bg_pinUpdate.RunWorkerAsync();
            t.Enabled = true;

        }

        private void analogPins_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            bg_pinUpdate.CancelAsync();
            t.Enabled = false;
            while (bg_pinUpdate.IsBusy) ;
        }

        #endregion

           
    }
}
