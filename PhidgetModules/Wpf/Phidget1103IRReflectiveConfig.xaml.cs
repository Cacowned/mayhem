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
using Phidgets;
using Phidgets.Events;
using System.Windows.Threading;

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
