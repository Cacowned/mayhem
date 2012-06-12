using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using System.Windows.Threading;
using MayhemWpf.UserControls;
using ArduinoModules;
using ArduinoModules.Events;

namespace ArduinoModules.Wpf
{
    /// <summary>
    /// Interaction logic for ArduinoDigitalInputConfig.xaml
    /// </summary>
    public partial class ArduinoDigitalInputConfig : WpfConfiguration
    {
        public DigitalInputType InputType { get; set; }
        public int Index { get; set; }
        public string Port { get; set; }
        public string[] Ports { get; set; }
        
        public ArduinoDigitalInputConfig (int index, DigitalInputType inputType, string port)
        {
            InputType = inputType;
            Index = index;
            Port = port;
            
            CanSave = true;            
            InitializeComponent();
        }

        public override void OnLoad()
        {
            PopulateOutputs();
            //CheckCanSave();

            InputBox.SelectedIndex = Index;
            
            switch (InputType)
            {
                case DigitalInputType.Toggle: ControlBox.SelectedIndex = 0;
                    break;
                case DigitalInputType.On: ControlBox.SelectedIndex = 1;
                    break;
                case DigitalInputType.Off: ControlBox.SelectedIndex = 2;
                    break;
            }
            
            PortBox.SelectedItem = Port;
        }

        private void PopulateOutputs()
        {
            //if (ifKit.Attached)
            //{
            //    Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            //    {
            //        for (int i = 0; i < ifKit.outputs.Count; i++)
            //        {
                           
            //        }
            //    }));
            //}
            
            // Add digital output pins as specified in MSP430 firmware
            InputBox.Items.Add("P1.3");        // LED1
            InputBox.Items.Add("P2.1");        // GPIO pin
            InputBox.Items.Add("P2.2");        // GPIO pin
            InputBox.Items.Add("P2.3");        // GPIO pin
            InputBox.Items.Add("P2.4");        // GPIO pin
            InputBox.Items.Add("P2.5");        // GPIO pin
            
            // Get serial ports available for selection
            string[] ports = SerialManager.SerialPortManager.AllPorts;

            if (ports.Length.Equals(0))
            {
                PortBox.Items.Add("There are no available COM ports");
            }

            else
            {
                foreach (string port in ports)
                {
                    PortBox.Items.Add(port);
                }
            }
        }

        private void InputBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            Index = box.SelectedIndex;
        }

        private void PortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            Port = box.SelectedItem.ToString();
            Console.Write(Port);
        }

        public override void OnSave()
        {
            Index = InputBox.SelectedIndex;

            ComboBoxItem inputBox = ControlBox.SelectedItem as ComboBoxItem;
            switch (inputBox.Content.ToString())
            {
                case "Toggles": InputType = DigitalInputType.Toggle;
                    break;
                case "Turns On": InputType = DigitalInputType.On;
                    break;
                case "Turns Off": InputType = DigitalInputType.Off;
                    break;
            }
            
            int selectedPort = PortBox.SelectedIndex;
            Port = SerialManager.SerialPortManager.AllPorts[selectedPort];            
        }

        public override string Title
        {
            get
            {
                return "Arduino: Digital Input";
            }
        }
    }
}
