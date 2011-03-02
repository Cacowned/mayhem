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
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IndexProperty =
			DependencyProperty.Register("Index", typeof(int), typeof(PhidgetDigitalOutputConfig), new UIPropertyMetadata(0));

		public InterfaceKit IfKit;

		public DigitalOutputType outputType;

		public bool defaultValue;

		public PhidgetDigitalOutputConfig(InterfaceKit ifKit, int index, bool startValue, DigitalOutputType outputType) {
			this.Index = index;
			this.IfKit = ifKit;

			this.defaultValue = startValue;
			this.outputType = outputType;

			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {

			for (int i = 0; i < IfKit.outputs.Count; i++) {
				OutputBox.Items.Add(i);
			}

			this.OutputBox.SelectedIndex = Index;

			this.ControlBox.SelectedIndex = 0;

			switch (outputType) {
				case DigitalOutputType.Toggle: this.ControlBox.SelectedIndex = 0;
					break;
				case DigitalOutputType.On: this.ControlBox.SelectedIndex = 1;
					break;
				case DigitalOutputType.Off: this.ControlBox.SelectedIndex = 2;
					break;
			}

			StartOn.IsChecked = defaultValue;
			StartOff.IsChecked = !defaultValue;


		}

		private void OutputBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			ComboBox box = sender as ComboBox;
			Index = box.SelectedIndex;
		}

		private void Button_Save_Click(object sender, RoutedEventArgs e) {
			Index = OutputBox.SelectedIndex;

			ComboBoxItem item = ControlBox.SelectedItem as ComboBoxItem;
			switch (item.Content.ToString()) {
				case "Toggle": outputType = DigitalOutputType.Toggle;
					break;
				case "Turn On": outputType = DigitalOutputType.On;
					break;
				case "Turn Off": outputType = DigitalOutputType.Off;
					break;
			}

			defaultValue = (bool)StartOn.IsChecked;

			DialogResult = true;

		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}


	}
}
