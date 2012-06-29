using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using MSP430Modules.Reactions;
using System.Diagnostics;
using System.Reflection;
using System.Management;
using MayhemCore;
using System.IO;

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
        public string PortName { get; set; }         

        public MSP430DigitalOutputConfig(int index, DigitalOutputType outputType, string port, string portName)
        {
            OutputType = outputType;
            Index = index;
            Port = port;
            PortName = portName;

            CanSave = true;                        
            InitializeComponent();
        }       

        public override void OnLoad()
        {            
            PopulateOutputs();           

            // Output:
            // Load the default output pin
            OutputBox.SelectedIndex = Index;

            // Trigger Type:
            // Load the default action
            switch (OutputType)
            {
                case DigitalOutputType.Toggle: ControlBox.SelectedIndex = 0;
                    break;
                case DigitalOutputType.High: ControlBox.SelectedIndex = 1;
                    break;
                case DigitalOutputType.Low: ControlBox.SelectedIndex = 2;
                    break;
            }

            // COM Port:
            // Load the COM port containing "MSP430" on first load
            if (Port == null)
            {
                foreach (object port in PortBox.Items)
                {
                    if (port.ToString().Contains("MSP430"))
                    {
                        PortName = port.ToString();
                        PortBox.SelectedItem = PortName;
                    }
                }
            }
            
            // Load the saved COM port on consecutive loads
            else
            {
                foreach (object port in PortBox.Items)
                {
                    if (port.ToString().Contains(Port))
                    {
                        PortName = port.ToString();
                        PortBox.SelectedItem = PortName;
                    }
                }
            }            
        }

        private void PopulateOutputs()
        {
            // Input:
            // Add digital output pins as specified in MSP430 firmware
            OutputBox.Items.Add("P1.0");        // LED1
            OutputBox.Items.Add("P1.4");
            OutputBox.Items.Add("P1.5");
            OutputBox.Items.Add("P1.6");        // LED2
            OutputBox.Items.Add("P1.7");
            OutputBox.Items.Add("P2.0");

            // Trigger Type:
            // The available actions are added in the .xaml file

            // COM Port:
            // Add COM port to the configuration                                   
            try
            {
                ObjectQuery query = new ObjectQuery("Select * from Win32_SerialPort");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                var result = searcher.Get();

                if (result != null)
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        PortBox.Items.Add(obj["DeviceID"] + " - " + obj["Description"]);                      
                    }
                }

                else
                {
                    PortBox.Items.Add("There are no available COM ports");
                }
            }

            catch
            {

            }
        }

        private void OutputBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Select the index of the output pin selected
            ComboBox box = sender as ComboBox;
            Index = box.SelectedIndex;
        }

        private void PortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Select the item for the COM Port selected
            ComboBox box = sender as ComboBox;
            PortName = box.SelectedItem.ToString();            
        }

        public override void OnSave()
        {
            // Output:
            // Save the selected output pin
            Index = OutputBox.SelectedIndex;

            // Trigger Type:
            // Save the selected action
            ComboBoxItem outputBox = ControlBox.SelectedItem as ComboBoxItem;
            switch (outputBox.Content.ToString())
            {
                case "Toggle": OutputType = DigitalOutputType.Toggle;
                    break;
                case "Turn High": OutputType = DigitalOutputType.High;
                    break;
                case "Turn Low": OutputType = DigitalOutputType.Low;
                    break;
            }

            // COM Port:
            // Split the COM Port description and save the "COM#" 
            PortName = PortBox.SelectedItem.ToString();
            string[] words = PortName.Split('-');
            Port = words[0].Trim();           
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
            Assembly execAssembly = Assembly.GetExecutingAssembly();
            string loc = execAssembly.Location;
            string directory = Path.GetDirectoryName(loc);
            string flasher = @"Flasher\FlashMSP430.bat";
            string flashBatch = Path.Combine(directory, flasher);            
            Process.Start(flashBatch);
        }
    }
}