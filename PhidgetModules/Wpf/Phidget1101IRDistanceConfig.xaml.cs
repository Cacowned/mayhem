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

	public partial class Phidget1101IRDistanceConfig : Window
	{
		public int index;
		public double topValue;

		public bool increasing;

		public InterfaceKit ifKit;

		protected SensorChangeEventHandler handler;

		protected Func<int, double> convertor;

		public Phidget1101IRDistanceConfig(InterfaceKit ifKit, int index, double topValue, bool increasing, Func<int, double> conversion) {
			this.index = index;
			this.ifKit = ifKit;
			this.topValue = topValue;
			this.convertor = conversion;

			InitializeComponent();

			handler = new SensorChangeEventHandler(SensorChange);

			for (int i = 0; i < ifKit.sensors.Count; i++) {
				SensorBox.Items.Add(i);
			}

			this.SensorBox.SelectedIndex = index;
			TopValue.Text = topValue.ToString();

			IncreasingRadio.IsChecked = increasing;
			DecreasingRadio.IsChecked = !increasing;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			this.ifKit.SensorChange += handler;
		}

		protected void SensorChange(object sender, SensorChangeEventArgs e) {
			this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
			{
				// We only care about the index we are looking at.
				if (e.Index == index) {

					if ((e.Value < 490) && (e.Value > 80)) {
						this.ValueBox.Text = convertor(e.Value).ToString("0.##") + " cm";
					} else {
						this.ValueBox.Text = "Object Not Detected";
					}

				}
			}));


		}

		private void Button_Save_Click(object sender, RoutedEventArgs e) {
			if (!double.TryParse(TopValue.Text, out topValue) && topValue >= 0) {
				MessageBox.Show("You must enter a valid number");
			} else {
				increasing = (bool)IncreasingRadio.IsChecked;
				DialogResult = true;
			}
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}

		private void SensorBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			ComboBox box = sender as ComboBox;

			index = box.SelectedIndex;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			this.ifKit.SensorChange -= handler;
		}
	}
}
