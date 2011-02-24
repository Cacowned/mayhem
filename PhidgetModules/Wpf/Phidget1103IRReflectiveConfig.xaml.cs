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


		public Phidget1103IRReflectiveConfig(InterfaceKit ifKit, int index, bool onTurnOn) {
			this.index = index;
			this.onTurnOn = onTurnOn;

			InitializeComponent();


			OnWhenOn.IsChecked = onTurnOn;
			OnWhenOff.IsChecked = !onTurnOn;
		}

        /*
		protected void SensorChange(object sender, SensorChangeEventArgs e) {
			this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
			{
				// We only care about the index we are looking at.
				if (e.Index == index) {
					this.ValueBox.Text = IsDetected(e.Value) ? "Detected" : "Not Detected";
				}
			}));
		}
         * */

		public bool IsDetected(int value) {
			if (value < 100)
				return true;
			return false;
		}

		private void Button_Save_Click(object sender, RoutedEventArgs e) {
			onTurnOn = (bool)OnWhenOn.IsChecked;
			DialogResult = true;
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}
	}
}
