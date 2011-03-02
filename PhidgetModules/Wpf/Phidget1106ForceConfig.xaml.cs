using System;
using System.Windows;
using Phidgets;

namespace PhidgetModules.Wpf
{
	public partial class Phidget1106ForceConfig : Window
	{
		public int index;
		public double topValue;
		public bool increasing;

		public InterfaceKit IfKit;
		protected Func<int, string> convertor;

		public Phidget1106ForceConfig(InterfaceKit ifKit, int index, double topValue, bool increasing, Func<int, string> conversion) {
			this.index = index;
			this.topValue = topValue;

			this.IfKit = ifKit;
			this.convertor = conversion;

			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			SensorDataBox.Index = index;
			SensorDataBox.IfKit = IfKit;
			SensorDataBox.convertor = convertor;

			TopValue.Text = topValue.ToString();

			IncreasingRadio.IsChecked = increasing;
			DecreasingRadio.IsChecked = !increasing;
		}

		private void Button_Save_Click(object sender, RoutedEventArgs e) {
			if (!double.TryParse(TopValue.Text, out topValue) && topValue >= 0) {
				MessageBox.Show("You must enter a valid number");
			} else {
				increasing = (bool)IncreasingRadio.IsChecked;
				index = SensorDataBox.Index;

				DialogResult = true;
			}
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}
	}
}
