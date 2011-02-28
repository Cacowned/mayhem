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
using PhidgetModules.Action;

namespace PhidgetModules.Wpf
{
	/// <summary>
	/// Interaction logic for _1133SoundConfig.xaml
	/// </summary>
	public partial class Phidget1133SoundConfig : Window
	{
		public int index;
		public double topValue;
		public bool increasing;

		public InterfaceKit ifKit;
		protected Func<int, string> convertor;

		public Phidget1133SoundConfig(InterfaceKit ifKit, int index, double topValue, bool increasing, Func<int, string> conversion) {
			this.index = index;
			this.topValue = topValue;

			this.ifKit = ifKit;
			this.convertor = conversion;

			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			SensorDataBox.Index = index;
			SensorDataBox.IfKit = ifKit;
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
