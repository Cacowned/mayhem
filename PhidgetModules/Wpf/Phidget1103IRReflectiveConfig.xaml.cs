using System;
using System.Windows;
using Phidgets;

namespace PhidgetModules.Wpf
{

	public partial class Phidget1103IRReflectiveConfig : Window
	{

		public int index;
		public bool onTurnOn;

		protected Func<int, string> convertor;
		public InterfaceKit IfKit;


		public Phidget1103IRReflectiveConfig(InterfaceKit ifKit, int index, bool onTurnOn, Func<int, string> conversion) {
			this.index = index;
			this.onTurnOn = onTurnOn;

			this.convertor = conversion;
			this.IfKit = ifKit;

			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			SensorDataBox.Index = index;
			SensorDataBox.IfKit = IfKit;
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
