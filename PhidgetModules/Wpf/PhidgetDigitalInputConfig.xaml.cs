using System.Windows.Controls;
using Phidgets;
using System.Windows;

namespace PhidgetModules.Wpf
{
	/// <summary>
	/// Interaction logic for PhidgetDigitalInputConfig.xaml
	/// </summary>
	public partial class PhidgetDigitalInputConfig : Window
	{
		public int Index { get; set; }

		public InterfaceKit IfKit { get; set; }

		// If true, then trigger when the digital input
		// turns on, otherwise when the digital input
		// turns off
		public bool OnWhenOn { get; set; }

		public PhidgetDigitalInputConfig(InterfaceKit ifKit, int index, bool onWhenOn) {
			this.Index = index;
			this.IfKit = ifKit;
			this.OnWhenOn = onWhenOn;

			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {

			for (int i = 0; i < IfKit.inputs.Count; i++) {
				InputBox.Items.Add(i);
			}

			this.InputBox.SelectedIndex = Index;

			GoesOnRadio.IsChecked = OnWhenOn;
			TurnsOffRadio.IsChecked = !OnWhenOn;
		}
		
		private void Button_Save_Click(object sender, RoutedEventArgs e) {
			Index = InputBox.SelectedIndex;

			OnWhenOn = (bool)GoesOnRadio.IsChecked;

			DialogResult = true;
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}
	}
}
