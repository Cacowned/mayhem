using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ArduinoModules.Firmata;
using ArduinoModules.Wpf.Helpers;
using MayhemCore;
using MayhemSerial;
using MayhemWpf.UserControls;

namespace ArduinoModules.Wpf
{
    /// <summary>
    /// Interaction logic for ArduinoEventConfig.xaml
    /// </summary>
    public partial class ArduinoEventConfig : WpfConfiguration, IArduinoEventListener
    {
        private readonly MayhemSerialPortMgr serial;
        private readonly Timer timer;
        private readonly List<DigitalPinItem> presetDigitalPins;
        private readonly List<AnalogPinItem> presetAnalogPins;
        private readonly BackgroundWorker bgPinUpdate;

        private int itemSelected;
        private Dictionary<string, string> deviceNamesIds;
        private ArduinoFirmata arduino;

        // collections driving the itemPanels
        public ObservableCollection<DigitalPinItem> DigitalPinItems
        {
            get;
            private set;
        }

        public ObservableCollection<AnalogPinItem> AnalogPinItems
        {
            get;
            private set;
        }

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

        public ArduinoEventConfig(List<DigitalPinItem> reactionDigitalPins, List<AnalogPinItem> reactionAnalogPins)
        {
            bgPinUpdate = new BackgroundWorker();
            timer = new Timer(1000);
            itemSelected = -1;
            serial = MayhemSerialPortMgr.Instance;
            presetDigitalPins = reactionDigitalPins;
            presetAnalogPins = reactionAnalogPins;

            DigitalPinItems = new ObservableCollection<DigitalPinItem>();
            AnalogPinItems = new ObservableCollection<AnalogPinItem>();
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

            bgPinUpdate.DoWork += (o, e) =>
            {
                Dispatcher.BeginInvoke(new Action(() => digitalPins.Items.Refresh()), DispatcherPriority.Render);
                Dispatcher.BeginInvoke(new Action(() => analogPins.Items.Refresh()), DispatcherPriority.Render);
            };

            bgPinUpdate.WorkerSupportsCancellation = true;

            timer.AutoReset = true;
            timer.Elapsed += TimerElapsed;
            timer.Enabled = true;

            CanSave = true;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!bgPinUpdate.IsBusy)
                bgPinUpdate.RunWorkerAsync();
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

            if (arduino != null && (string)box.SelectedValue != arduino.PortName)
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

            timer.Enabled = false;
            bgPinUpdate.CancelAsync();
            bgPinUpdate.Dispose();
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
            Dispatcher.Invoke(new Action(() =>
            {
                connectButton.Content = "Connected";
                connectButton.IsEnabled = false;
            }), null);
        }

        public void Arduino_OnAnalogPinChanged(Pin p)
        {
            if (p.AnalogChannel < AnalogPinItems.Count)
            {
                // refresh data grid items
                AnalogPinItems[p.AnalogChannel].SetAnalogValue(p.Value);
            }
        }

        public void Arduino_OnDigitalPinChanged(Pin p)
        {
            Logger.WriteLine("arduino_OnDigitalPinChanged " + p.Id + " " + p.Value);

            if (p.Id < DigitalPinItems.Count)
            {
                foreach (DigitalPinItem pin in DigitalPinItems)
                {
                    if (pin.GetPinId() == p.Id)
                        pin.SetPinState(p.Value);
                }
            }
        }

        public void Arduino_OnPinAdded(Pin p)
        {
            Logger.WriteLine("arduino_OnPinAdded: " + p.Id);

            if (p.Mode != PinMode.Analog &&
                p.Mode != PinMode.Unassigned &&
                p.Mode != PinMode.Shift)
            {
                DigitalPinItem pItem = new DigitalPinItem(false, p.Id, DigitalPinChange.Low);
                foreach (DigitalPinItem setPin in presetDigitalPins)
                {
                    if (setPin.GetPinId() == p.Id)
                    {
                        pItem = setPin;
                        Logger.WriteLine("Using already configured pin " + setPin.GetPinId());
                    }
                }

                Dispatcher.Invoke(new Action(() => { DigitalPinItems.Add(pItem); }), null);

                // set digital pin mode to input
                if (p.Mode != PinMode.Input && p.Flagged == false)
                {
                    arduino.SetPinMode(p, PinMode.Input);
                }
            }
            else if (p.Mode == PinMode.Analog)
            {
                AnalogPinItem aItem = new AnalogPinItem(false, p.Id, AnalogPinChange.Equal);
                foreach (AnalogPinItem setPin in presetAnalogPins)
                {
                    if (setPin.GetPinId() == p.Id)
                    {
                        aItem = setPin;
                        Logger.WriteLine("Using already configured pin " + setPin.GetPinId());
                    }
                }

                Dispatcher.Invoke(new Action(() => { AnalogPinItems.Add(aItem); }), null);
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
            timer.Enabled = false;
            while (bgPinUpdate.IsBusy)
            {
            }

            (sender as DataGrid).RowEditEnding -= digitalPins_RowEditEnding;
            (sender as DataGrid).CommitEdit();
            (sender as DataGrid).RowEditEnding += digitalPins_RowEditEnding;

            bgPinUpdate.RunWorkerAsync();
            timer.Enabled = true;
        }

        private void digitalPins_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            bgPinUpdate.CancelAsync();
            timer.Enabled = false;
            while (bgPinUpdate.IsBusy)
            {
            }
        }

        private void analogPins_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            bgPinUpdate.CancelAsync();
            timer.Enabled = false;
            while (bgPinUpdate.IsBusy)
            {
            }

            (sender as DataGrid).RowEditEnding -= analogPins_RowEditEnding;
            (sender as DataGrid).CommitEdit();
            (sender as DataGrid).RowEditEnding += analogPins_RowEditEnding;

            bgPinUpdate.RunWorkerAsync();
            timer.Enabled = true;
        }

        private void analogPins_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            bgPinUpdate.CancelAsync();
            timer.Enabled = false;
            while (bgPinUpdate.IsBusy)
            {
            }
        }
        #endregion
    }
}
