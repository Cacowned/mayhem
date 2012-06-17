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
using MSP430Modules;
using MSP430Modules.Events;
using MSP430Modules.Reactions;

namespace MSP430Modules.Wpf
{
    /// <summary>
    /// Interaction logic for MSP430DigitalOutputConfig.xaml
    /// </summary>
    public partial class MSP430DigitalOutputConfig : WpfConfiguration
    {
        public DigitalOutputType OutputType { get; set; }
        public int Index { get; set; }
        public string Port { get; set; }
        public string[] Ports { get; set; }
        
        public MSP430DigitalOutputConfig (int index, DigitalOutputType outputType, string port)
        {
            OutputType = outputType;
            Index = index;
            Port = port;
            
            CanSave = true;            
            InitializeComponent();
        }

        public override void OnLoad()
        {
            PopulateOutputs();
            //CheckCanSave();

            OutputBox.SelectedIndex = Index;
            
            switch (OutputType)
            {
                case DigitalOutputType.Toggle: ControlBox.SelectedIndex = 0;
                    break;
                case DigitalOutputType.On: ControlBox.SelectedIndex = 1;
                    break;
                case DigitalOutputType.Off: ControlBox.SelectedIndex = 2;
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
            OutputBox.Items.Add("P1.0");        // LED1
            OutputBox.Items.Add("P1.4");        // GPIO pin
            OutputBox.Items.Add("P1.5");        // GPIO pin
            OutputBox.Items.Add("P1.6");        // GPIO pin
            OutputBox.Items.Add("P1.7");        // GPIO pin
            OutputBox.Items.Add("P2.0");        // GPIO pin
            
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

        private void OutputBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            Index = OutputBox.SelectedIndex;

            ComboBoxItem outputBox = ControlBox.SelectedItem as ComboBoxItem;
            switch (outputBox.Content.ToString())
            {
                case "Toggle": OutputType = DigitalOutputType.Toggle;
                    break;
                case "Turn On": OutputType = DigitalOutputType.On;
                    break;
                case "Turn Off": OutputType = DigitalOutputType.Off;
                    break;
            }
            
            int selectedPort = PortBox.SelectedIndex;
            Port = SerialManager.SerialPortManager.AllPorts[selectedPort];            
        }

        public override string Title
        {
            get
            {
                return "MSP430: Digital Output";
            }
        }

        private void FlashButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"C:\Users\t-greyes\Desktop\Mayhem\Modules\MSP430Modulesv2\Flasher\FlashMSP430.bat");
        }
    }
}
