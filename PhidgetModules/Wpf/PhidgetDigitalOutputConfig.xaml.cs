using System.Windows;
using System.Windows.Controls;
using PhidgetModules.Reaction;
using Phidgets;

namespace PhidgetModules.Wpf
{
	/// <summary>
	/// Interaction logic for PhidgetDigitalOutputConfig.xaml
	/// </summary>
	public partial class PhidgetDigitalOutputConfig : Window
	{
		public int Index { get; set; }

		public InterfaceKit IfKit { get; set; }

		public DigitalOutputType OutputType { get; set; }
		
		public PhidgetDigitalOutputConfig(InterfaceKit ifKit, int index, DigitalOutputType outputType) {
			this.Index = index;
			this.IfKit = ifKit;

			this.OutputType = outputType;

			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {

			for (int i = 0; i < IfKit.outputs.Count; i++) {
				OutputBox.Items.Add(i);
			}

			this.OutputBox.SelectedIndex = Index;

			this.ControlBox.SelectedIndex = 0;

			switch (OutputType) {
				case DigitalOutputType.Toggle: this.ControlBox.SelectedIndex = 0;
					break;
				case DigitalOutputType.On: this.ControlBox.SelectedIndex = 1;
					break;
				case DigitalOutputType.Off: this.ControlBox.SelectedIndex = 2;
					break;
			}
		}

		private void OutputBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			ComboBox box = sender as ComboBox;
			Index = box.SelectedIndex;
		}

		private void Button_Save_Click(object sender, RoutedEventArgs e) {
			Index = OutputBox.SelectedIndex;

			ComboBoxItem item = ControlBox.SelectedItem as ComboBoxItem;
			switch (item.Content.ToString()) {
				case "Toggle": OutputType = DigitalOutputType.Toggle;
					break;
				case "Turn On": OutputType = DigitalOutputType.On;
					break;
				case "Turn Off": OutputType = DigitalOutputType.Off;
					break;
			}

			DialogResult = true;
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}
	}
}
