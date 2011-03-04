using System.Windows;
using System.Windows.Forms;


namespace VisionModules.Wpf
{
	/// <summary>
	/// Interaction logic for WebcamSnapshotConfig.xaml
	/// </summary>
	public partial class CamSnapshotConfig : Window
	{
		public string location;
		// public Device captureDevice;



		public CamSnapshotConfig(string location, object captureDevice) {
			this.location = location;
			

			//InitializeComponent();

            // TODO: Enumerate devices
	

		}


		private void Button_Click_1(object sender, RoutedEventArgs e) {
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.SelectedPath = location;

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				location = dlg.SelectedPath;
			}
		}

		private void Button_Save_Click(object sender, RoutedEventArgs e) {
			// captureDevice = DeviceList.SelectedItem as Device;
			DialogResult = true;
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}
	}
}
