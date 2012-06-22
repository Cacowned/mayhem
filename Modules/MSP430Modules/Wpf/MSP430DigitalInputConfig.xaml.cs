using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using MSP430Modules.Events;
using System.Reflection;
using System.Diagnostics;
using System.Management;

namespace MSP430Modules.Wpf
{
    /// <summary>
    /// Interaction logic for MSP430DigitalInputConfig.xaml
    /// </summary>
    public partial class MSP430DigitalInputConfig : WpfConfiguration
    {
        public DigitalInputType InputType { get; set; }
        public int Index {  get; set; }
        public string Port { get; set; }
        public string PortName { get; set; }

        public MSP430DigitalInputConfig(int index, DigitalInputType inputType, string port, string portName)
        {
            InputType = inputType;
            Index = index;
            Port = port;
            PortName = portName;

            CanSave = true;
            InitializeComponent();
        }

        public override void OnLoad()
        {
            PopulateOutputs();

            // Input:
            // Load the default input pin
            InputBox.SelectedIndex = Index;

            // Trigger Type:
            // Load the default action
            switch (InputType)
            {
                case DigitalInputType.Toggle: ControlBox.SelectedIndex = 0;
                    break;
                case DigitalInputType.High: ControlBox.SelectedIndex = 1;
                    break;
                case DigitalInputType.Low: ControlBox.SelectedIndex = 2;
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

        public void PopulateOutputs()
        {
            // Input:
            // Add digital input pins as specified in MSP430 firmware
            InputBox.Items.Add("P1.3");        
            InputBox.Items.Add("P2.1");        
            InputBox.Items.Add("P2.2");        
            InputBox.Items.Add("P2.3");        
            InputBox.Items.Add("P2.4");        
            InputBox.Items.Add("P2.5");        

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

        private void InputBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Select the index of the input pin selected
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
            // Input:
            // Save the selected input pin
            Index = InputBox.SelectedIndex;

            // Trigger Type:
            // Save the selected action
            ComboBoxItem inputBox = ControlBox.SelectedItem as ComboBoxItem;
            switch (inputBox.Content.ToString())
            {
                case "Toggles": InputType = DigitalInputType.Toggle;
                    break;
                case "Turns High": InputType = DigitalInputType.High;
                    break;
                case "Turns Low": InputType = DigitalInputType.Low;
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
                return "MSP430: Digital Input";
            }
        }

        private void FlashButton_Click(object sender, RoutedEventArgs e)
        {
            Assembly execAssembly = Assembly.GetExecutingAssembly();
            string loc = execAssembly.Location;
            string directory = System.IO.Path.GetDirectoryName(loc);
            Process.Start(@directory + "/Flasher/FlashMSP430.bat");
        }
    }
}