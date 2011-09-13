/*
 * InsteonReactionConfig.xaml.cs
 * 
 * 
 * User interface logic for the InsteonReaction Module
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
using X10Modules.Insteon;
using System.Diagnostics;
using System.Threading;

namespace X10Modules.Wpf
{
    /// <summary>
    /// Interaction logic for InsteonReactionConfig.xaml
    /// </summary>
    public partial class InsteonReactionConfig : IWpfConfiguration
    {
        public static readonly string TAG = "[InsteonReactionConfig] : ";
        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.instance;
        private InsteonController insteonController = null;

        private bool linking = false;

        public byte[] selected_device_address
        {
            get
            {
                byte id0 = byte.Parse(devAddr0.Text, System.Globalization.NumberStyles.AllowHexSpecifier);
                byte id1 = byte.Parse(devAddr1.Text, System.Globalization.NumberStyles.AllowHexSpecifier);
                byte id2 = byte.Parse(devAddr2.Text, System.Globalization.NumberStyles.AllowHexSpecifier);

                byte[] address = new byte[3] { id0, id1, id2 };
                return address;

            }
        }

        public string selected_portname
        {
            get
            {
                return (string) deviceList.SelectedValue; 
            }
        }

        public byte selected_command
        {
            get
            {
                return selectable_commands[(string)commandID.SelectedItem][0];
            }
        }


        // define the available commands
        // TODO: Evaluate in future if byte array is needed or not
        private Dictionary<string, byte[]> selectable_commands = new Dictionary<string, byte[]>()
        {
            { "ON",  new byte[]{InsteonCommandBytes.light_on_fast}},
            { "OFF", new byte[]{InsteonCommandBytes.light_off_fast}},
            { "TOGGLE", new byte[] {InsteonCommandBytes._toggle}}
        };

        public InsteonReactionConfig()
        {
            InitializeComponent();
            Init();
        }

        /// <summary>
        /// Initialize the config
        /// </summary>
        public void Init()
        {
            serial = MayhemSerialPortMgr.instance; 

            serial.UpdatePortList();
            //deviceList.ItemsSource = serial.serialPortNames;

            Dictionary<string,string> portList = serial.getInsteonPortNames();

            if (portList.Count > 0)
            {
                deviceList.ItemsSource = portList;
                deviceList.DisplayMemberPath = "Value";
                deviceList.SelectedValuePath = "Key";
                deviceList.SelectedIndex = 0;

                // if the device list has more than one entry, also enumerate the detected devices on that port

                insteonController = new InsteonController((string) deviceList.SelectedValue);
                if (insteonController.initialized)
                {
                    // retrieve device lists from controller
                    List<InsteonDevice> devices = insteonController.EnumerateLinkedDevices();

                    cbox_link_devices.ItemsSource = devices;
                    cbox_link_devices.DisplayMemberPath = "ListName";
                    cbox_link_devices.SelectedValuePath = "deviceID";
                    cbox_link_devices.SelectedIndex = 0;
                }

            }
            commandID.ItemsSource = selectable_commands.Keys;
            commandID.SelectedIndex = 0;

            
          
        }

        /// <summary>
        /// Test Commands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(TAG + "Button_Click");
            string portname = (string) deviceList.SelectedValue;
            if (insteonController == null)
            {
                insteonController = new InsteonController(portname);
            }
            if (insteonController.initialized)
            {
                Debug.WriteLine(TAG + "insteon initialized");


                byte[] address = selected_device_address;

                byte command_code = selectable_commands[(string) commandID.SelectedItem][0];

                InsteonStandardMessage m = new InsteonStandardMessage(address, command_code);

                // execute command
                new Thread(
                   new ThreadStart(() => insteonController.SendStandardMsg(m))).Start();
                //insteonController.SendStandardMsg(m);


            }
        }

        /// <summary>
        /// Allows the user to set the Insteon Modem to enter / exit all-link mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_link_devices_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(TAG + "btn_link_devices_Click");
            string portname = (string)deviceList.SelectedValue;
            btn_link_devices.Content = "Stop Linking";
            if (insteonController == null)
            {
                insteonController = new InsteonController(portname);
            }

            if (insteonController.initialized)
            {
                Debug.WriteLine("Insteon Initialized");
                

                byte[] address = selected_device_address;
            }

            if (!linking)
            {

               
                if (insteonController.initialized)
                {
                    linking = true; 
                    // send start linking command

                    if (insteonController.startAllLinking())
                    {
                        return;
                    }
                    else
                    {
                        // create a callback that resets the button text
                        TimerCallback cb = (S) => { btn_link_devices.Content = "Link More Devices"; };
                        Timer t = new Timer(cb, null, 2500, 0);
                    }

                }
            }
            else
            {
                linking = false;
                btn_link_devices.Content = "Link Devices";

                if (insteonController.initialized)
                {
                    // stop all linking
                    insteonController.stopAllLinking(); 

                    // retrieve device lists from controller
                    List<InsteonDevice> devices = insteonController.EnumerateLinkedDevices();

                    cbox_link_devices.ItemsSource = devices;
                    cbox_link_devices.DisplayMemberPath = "ListName";
                    cbox_link_devices.SelectedValuePath = "deviceID";
                    cbox_link_devices.SelectedIndex = 0;
                }
            }
  
        }

     

        /// <summary>
        /// Gets called when a different device is selected
        /// Updates the entries of the address fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_link_devices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
            byte[] selection = cbox_link_devices.SelectedValue as byte[];
            Debug.WriteLine(TAG + "cbox_link_devices_selectionChanged " +
             String.Format("{0:x2}:{1:x2}:{2:x2}", selection[0], selection[1], selection[2]));

            devAddr0.Text = String.Format("{0:x2}", selection[0]);
            devAddr1.Text = String.Format("{0:x2}", selection[1]);
            devAddr2.Text = String.Format("{0:x2}", selection[2]);
        }

        /// <summary>
        /// Gets called when a differnet COM port is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected_portname = deviceList.SelectedItem as string; 
        }



    }
}
