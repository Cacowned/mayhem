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
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Wpf
{

	public partial class Phidget1101IRDistanceConfig : Window
	{
		public int index;
		public double topValue;

        public double bottomValue;

		protected Func<int, string> convertor;

		public InterfaceKit IfKit;


		public Phidget1101IRDistanceConfig(InterfaceKit ifKit, int index, double topValue, double bottomValue, Func<int, string> conversion) {
			this.topValue = topValue;
            this.bottomValue = bottomValue;
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
			BottomValue.Text = bottomValue.ToString();

		}

		private void Button_Save_Click(object sender, RoutedEventArgs e) {
			if (!double.TryParse(TopValue.Text, out topValue) && topValue >= 0) {
				MessageBox.Show("You must enter a valid number for the top of the range");
			} else if(!double.TryParse(BottomValue.Text, out bottomValue) && topValue >= 0) {
                MessageBox.Show("You must enter a valid number for the bottom of the range");
			} else if (bottomValue > topValue) {
                MessageBox.Show("The bottom of the range must be lower than the top of the range");
            }
            else
            {
                DialogResult = true;
            }
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}
	}
}
