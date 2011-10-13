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
using MayhemWpf.UserControls;
using MayhemSerial;

namespace ArduinoModules.Wpf
{
    /// <summary>
    /// Interaction logic for MayduinoEvent.xaml
    /// </summary>

    public partial class MayduinoEventConfig : WpfConfiguration
    {
        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.Instance;
        private Dictionary<string, string> deviceNamesIds = null;


        public int Pin
        {
            get { return (int) bx_pin.SelectedValue; }
        }

        public T GetCondition<T>()
        {
           return (T) bx_cond.SelectedValue;
        }

        public T GetPullup<T>()
        {
            return (T)bx_pullup.SelectedValue;
        }

       

        public int ActivationValue
        {
            get
            {
                int value = 0;
                try
                {
                    value = int.Parse(bx_eventVals.Text);

                }
                catch
                {
                    value = 0; 
                }
                return value;
            }
        }


        public string ArduinoPortName
        {
            get;
            private set;
        }

        public MayduinoEventConfig()
        {
            InitializeComponent();
        }

        public override void OnLoad()
        {
           
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
            FillSelections();
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            ArduinoPortName  = (string)deviceList.SelectedValue;              
        }

        public virtual void FillSelections()
        {
            throw new NotImplementedException();
        }

        private void deviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ArduinoPortName = (string)deviceList.SelectedValue;    
        }
    }
}
