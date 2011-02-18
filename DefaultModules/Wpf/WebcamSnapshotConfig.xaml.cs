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
using DefaultModules.WebcamHelpers;
using System.Windows.Forms;
using System.Windows.Interop;

namespace DefaultModules.Wpf
{
    /// <summary>
    /// Interaction logic for WebcamSnapshotConfig.xaml
    /// </summary>
    public partial class WebcamSnapshotConfig : Window
    {
        public string location;
        public Device captureDevice;

        

        public WebcamSnapshotConfig(string location, Device captureDevice) {
            this.location = location;
            this.captureDevice = captureDevice;

            InitializeComponent();

            // Fill the device box
            foreach (Device device in Device.FindDevices())
                DeviceList.Items.Add(device);

            DeviceList.SelectedIndex = 0;
            
        }


        private void Button_Click_1(object sender, RoutedEventArgs e) {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = location;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                location = dlg.SelectedPath;
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e) {
            captureDevice = DeviceList.SelectedItem as Device;
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}
