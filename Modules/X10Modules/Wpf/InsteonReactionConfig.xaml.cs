﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using MayhemCore;
using MayhemSerial;
using MayhemWpf.UserControls;
using X10Modules.Insteon;
using X10Modules.Insteon.InsteonCommands;

namespace X10Modules.Wpf
{
    /// <summary>
    /// User interface logic for the InsteonReaction Module
    /// </summary>
    public partial class InsteonReactionConfig : WpfConfiguration
    {
        // define the available commands
        // TODO: Evaluate in future if byte array is needed or not
        private readonly Dictionary<string, byte[]> selectableCommands = new Dictionary<string, byte[]>
        {
            { "ON",  new[] { InsteonCommandBytes.LightOnFast } },
            { "OFF", new[] { InsteonCommandBytes.LightOffFast } },
            { "TOGGLE", new[] { InsteonCommandBytes.Toggle } }
        };

        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.Instance;
        private InsteonController insteonController;

        private bool linking;

        public byte[] SelectedDeviceAddress
        {
            get
            {
                byte id0 = byte.Parse(devAddr0.Text, System.Globalization.NumberStyles.AllowHexSpecifier);
                byte id1 = byte.Parse(devAddr1.Text, System.Globalization.NumberStyles.AllowHexSpecifier);
                byte id2 = byte.Parse(devAddr2.Text, System.Globalization.NumberStyles.AllowHexSpecifier);

                byte[] address = new[] { id0, id1, id2 };

                return address;
            }
        }

        public string SelectedPortname
        {
            get
            {
                return (string)deviceList.SelectedValue;
            }
        }

        public byte SelectedCommand
        {
            get
            {
                return selectableCommands[(string)commandID.SelectedItem][0];
            }
        }

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
            serial = MayhemSerialPortMgr.Instance;
            serial.UpdatePortList();

            Dictionary<string, string> portList = serial.GetInsteonPortNames(new InsteonUsbModemSerialSettings());

            if (portList.Count > 0)
            {
                deviceList.ItemsSource = portList;
                deviceList.DisplayMemberPath = "Value";
                deviceList.SelectedValuePath = "Key";
                deviceList.SelectedIndex = 0;

                // if the device list has more than one entry, also enumerate the detected devices on that port
                insteonController = InsteonController.ControllerForPortName((string)deviceList.SelectedValue);

                if (insteonController.Initialized)
                {
                    insteonController.StartAllLinking();
                    Thread.Sleep(100);
                    insteonController.stopAllLinking();

                    // retrieve device lists from controller
                    List<InsteonDevice> devices = insteonController.EnumerateLinkedDevices();

                    cbox_link_devices.ItemsSource = devices;
                    cbox_link_devices.DisplayMemberPath = "ListName";
                    cbox_link_devices.SelectedValuePath = "deviceID";
                    cbox_link_devices.SelectedIndex = 0;
                }
            }

            commandID.ItemsSource = selectableCommands.Keys;
            commandID.SelectedIndex = 0;
            CanSave = true;
        }

        /// <summary>
        /// Test Commands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Logger.WriteLine("Button_Click");
            string portname = (string)deviceList.SelectedValue;
            if (insteonController == null)
            {
                insteonController = InsteonController.ControllerForPortName(portname);
            }

            if (insteonController.Initialized)
            {
                Logger.WriteLine("insteon initialized");

                byte[] address = SelectedDeviceAddress;

                byte commandCode = selectableCommands[(string)commandID.SelectedItem][0];

                InsteonStandardMessage m = new InsteonStandardMessage(address, commandCode);

                // execute command
                new Thread(() => insteonController.SendStandardMsg(m)).Start();
            }
        }

        /// <summary>
        /// Allows the user to set the Insteon Modem to enter / exit all-link mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_link_devices_Click(object sender, RoutedEventArgs e)
        {
            // cancel the linking process
            Action cancelAllLink =
                            () =>
                            {
                                linking = false;
                                btn_link_devices.Content = "Link Devices";
                                if (insteonController.Initialized)
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
                            };

            Logger.WriteLine("btn_link_devices_Click");
            string portname = (string)deviceList.SelectedValue;
            btn_link_devices.Content = "Cancel Linking";
            if (insteonController == null)
            {
                insteonController = InsteonController.ControllerForPortName(portname);
            }

            if (insteonController.Initialized)
            {
                Logger.WriteLine("Insteon Initialized");
            }

            if (!linking)
            {
                if (insteonController.Initialized)
                {
                    linking = true;

                    // send start linking command
                    if (insteonController.StartAllLinking())
                    {
                        // create a callback that resets the and cancels the linking process                  
                        System.Timers.Timer t = new System.Timers.Timer(10000);
                        t.AutoReset = false;
                        t.Elapsed += (o, ea) => Dispatcher.Invoke(cancelAllLink);
                        t.Enabled = true;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                cancelAllLink();
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
            if (cbox_link_devices.SelectedValue != null)
            {
                byte[] selection = cbox_link_devices.SelectedValue as byte[];
                Logger.WriteLine("cbox_link_devices_selectionChanged " +
                 string.Format("{0:x2}:{1:x2}:{2:x2}", selection[0], selection[1], selection[2]));

                devAddr0.Text = string.Format("{0:x2}", selection[0]);
                devAddr1.Text = string.Format("{0:x2}", selection[1]);
                devAddr2.Text = string.Format("{0:x2}", selection[2]);
            }
        }

        /// <summary>
        /// Gets called when a differnet COM port is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedPortname = deviceList.SelectedItem as string;
        }

        public override string Title
        {
            get
            {
                return "Insteon Reaction";
            }
        }
    }
}
