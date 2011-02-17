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
        public int captureDevice;


        public WebcamSnapshotConfig(string location, int captureDevice) {
            this.location = location;
            this.captureDevice = captureDevice;

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

            WindowInteropHelper helper = new WindowInteropHelper(this);
            // setup a capture window
            captureDevice = Webcam.capCreateCaptureWindowA(lpszWindowName: "WebCap",
                                                dwStyle: 0,
                                                X: 0,
                                                Y: 0,
                                                nWidth: Webcam.Width,
                                                nHeight: Webcam.Height,
                                                hwndParent: helper.Handle.ToInt32(),
                                                nID: 0);

        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = location;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                location = dlg.SelectedPath;
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}
