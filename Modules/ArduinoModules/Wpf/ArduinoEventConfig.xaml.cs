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
using System.Timers; 

namespace ArduinoModules.Wpf
{

    /// <summary>
    /// Interaction logic for ArduinoEventConfig.xaml
    /// </summary>
    public partial class ArduinoEventConfig : IWpfConfiguration
    {
        public static string TAG = "[ArduinoEventConfig] :";
        
        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.instance;
        private int itemSelected = -1;
        private Dictionary<string, string>  deviceNamesIds = null;
        private ArduinoFirmata arduino = null;

        // collections driving the itemPanels
        public ObservableCollection<DigitalPinItem> digital_pin_items = new ObservableCollection<DigitalPinItem>();
        public ObservableCollection<AnalogPinItem> analog_pin_items = new ObservableCollection<AnalogPinItem>();


        Timer t = new Timer(1000);




        private BackgroundWorker bg_pinUpdate = new BackgroundWorker();

        public string arduinoPortName
        {
            get
            {
                if (arduino != null)
                {
                    return arduino.portName;
                }
                return null;

            }
        }

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
       


            bg_pinUpdate.DoWork += new DoWorkEventHandler((object o, DoWorkEventArgs e) => 
            
            {
                Dispatcher.BeginInvoke(new Action(() => {digitalPins.Items.Refresh();}),  DispatcherPriority.Render);
                Dispatcher.BeginInvoke(new Action(() => {analogPins.Items.Refresh(); }), DispatcherPriority.Render);
            });

            bg_pinUpdate.WorkerSupportsCancellation = true;

            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            t.Enabled = true;
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            //throw new NotImplementedException();
            if (!bg_pinUpdate.IsBusy)
                bg_pinUpdate.RunWorkerAsync();
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

        /// <summary>
        /// Executes intial connection with the Arduino Board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string portname = (string)deviceList.SelectedValue;

            if (arduino == null)
                arduino = ArduinoFirmata.InstanceForPortname(portname);
           
            arduino.OnPinAdded += new Action<Pin>(arduino_OnPinAdded);
            arduino.OnDigitalPinChanged += new Action<Pin>(arduino_OnDigitalPinChanged);
            arduino.OnAnalogPinChanged += new Action<Pin>(arduino_OnAnalogPinChanged);
            arduino.OnInitialized += new EventHandler(arduino_OnInitialized);
        }

        #region IWpfConfigurable overrides
        public override void OnClosing()
        {
            t.Enabled = false;
            base.OnClosing();
        }

        public override string Title
        {
            get
            {
                return "Arduino Event";
            }
        }
        #endregion


        #region Arduino Event Handlers

        void arduino_OnInitialized(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Dispatcher.Invoke(new Action(() =>
            {
                connectButton.Content = "Connected";
                connectButton.IsEnabled = false;
            }), null);

        }

        void arduino_OnAnalogPinChanged(Pin p)
        {
            //throw new NotImplementedException();
            //Debug.WriteLine(TAG+ "arduino_OnAnalogPinChanged: " + p.analog_channel + " v:"+p.value );
            if (p.analog_channel < analog_pin_items.Count)
            {
               
                // refresh data grid items
                analog_pin_items[p.analog_channel].SetAnalogValue(p.value);
                //if (!bg_pinUpdate.IsBusy)
                //    bg_pinUpdate.RunWorkerAsync();
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
               
                foreach (DigitalPinItem pin in digital_pin_items)
                {
                    if (pin.GetPinID() == p.id)
                        pin.SetPinState(p.value);
                }
             
              // Dispatcher.BeginInvoke(new Action(() => {digitalPins.Items.Refresh();}),DispatcherPriority.Render);
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

       

        
   
    }
}
