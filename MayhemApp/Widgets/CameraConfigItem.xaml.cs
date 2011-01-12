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
using MayhemOpenCVWrapper;
using System.Diagnostics;
using System.Threading;

namespace MayhemApp.Widgets
{
    /// <summary>
    /// Interaction logic for CameraConfigItem.xaml
    /// </summary>
    public partial class CameraConfigItem : ListBoxItem
    {

        public const string TAG = "[CameraConfigItem] :";


        // data binding for the camera names 
        public List<CameraInfo> cameraItems = null;


        public List<string> names = new List<string>();

        /*
        public List<string> names
        {
            get
            {
                return _names;
            }

            set
            {
                _names = value;
            }
        }*/
       

        public int selectedIndex = 0;

        public CameraConfigItem()
        {
            InitializeComponent();
            List<CameraInfo> items = new List<CameraInfo>();
            for (int i = 0; i < MayhemImageUpdater.Instance.devices_available.Length; i++)
            {
                CameraInfo c = MayhemImageUpdater.Instance.devices_available[i];
                items.Add(c);
                names.Add(c.deviceId + " : " + c.description);
            }         
            cameraBox.DataContext = names;
            cameraBox.ItemsSource = names;

            if (names.Count > 0)
            {
                cameraBox.SelectedIndex = MayhemImageUpdater.Instance.selected_device.deviceId;
            }
            else
            {
                cameraBox.SelectedIndex = 0;
            }
        }

        private void ListBoxItem_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

           
            // update the selected index to the currently selected camera

            if (MayhemImageUpdater.Instance.selected_device != null)
            {
                selectedIndex = MayhemImageUpdater.Instance.selected_device.deviceId;
            }
            else
            {
                selectedIndex = 0;
            }

            cameraBox.SelectedIndex = selectedIndex;

        }

        private void cameraBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(TAG + "camaeraBoxSelectionChanged " + cameraBox.SelectedIndex);

            if (cameraBox.SelectedIndex != selectedIndex)
            {
                // we change the camera
                Debug.WriteLine(TAG + "Will Stop Device: " + MayhemImageUpdater.Instance.selected_device.description);
                MayhemImageUpdater.Instance.Stop();
                MayhemImageUpdater.Instance.ReleaseDevice();

                // just for safety
                Thread.Sleep(150);

                Debug.WriteLine(TAG + "Will Start Device: " + cameraBox.SelectedItem);
                MayhemImageUpdater.Instance.InitCaptureDevice(cameraBox.SelectedIndex, 320, 240);

                
            }

            Debug.WriteLine(TAG + "camerabox_SelectionChanged ... done");

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO
            Debug.WriteLine(TAG + "ComboBox_SelectionChanged");
        }
    }
}
