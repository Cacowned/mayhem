using System;
using System.Windows;
using Phidgets;

namespace PhidgetModules.Wpf
{
	/// <summary>
	/// Interaction logic for Phidget1129TouchConfig.xaml
	/// </summary>
	public partial class Phidget1129TouchConfig : Window
	{
		public int index;
		public bool onTurnOn;

		public InterfaceKit ifKit;
		protected Func<int, string> convertor;

		public Phidget1129TouchConfig(InterfaceKit ifKit, int index, bool onTurnOn, Func<int, string> conversion) {
			this.index = index;
			this.ifKit = ifKit;
			this.onTurnOn = onTurnOn;
			this.convertor = conversion;

			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			SensorDataBox.Index = index;
			SensorDataBox.IfKit = ifKit;
			SensorDataBox.convertor = convertor;

			OnWhenOn.IsChecked = onTurnOn;
			OnWhenOff.IsChecked = !onTurnOn;
		}

		private void Button_Save_Click(object sender, RoutedEventArgs e) {
			onTurnOn = (bool)OnWhenOn.IsChecked;
			index = SensorDataBox.Index;

			DialogResult = true;
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}
	}
}
