using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MayhemWpf.UserControls;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Wpf
{
	public partial class Phidget1045TemperatureConfig : WpfConfiguration
	{
		private TemperatureSensor sensor;

		public double TopValue
		{
			get;
			private set;
		}

		public bool Increasing
		{
			get;
			private set;
		}

		public Phidget1045TemperatureConfig(double topValue, bool increasing)
		{
			TopValue = topValue;
			Increasing = increasing;

			InitializeComponent();
		}

		public override void OnLoad()
		{
			sensor = PhidgetManager.Get<TemperatureSensor>(false);
			sensor.Attach += sensor_Attach;
			sensor.Detach += sensor_Detach;
			sensor.TemperatureChange += sensor_TemperatureChange;

			textBoxTopValue.Text = TopValue.ToString();

			IncreasingRadio.IsChecked = Increasing;
			DecreasingRadio.IsChecked = !Increasing;

			SetAttached();
			CheckCanSave();
		}

		private void sensor_TemperatureChange(object sender, TemperatureChangeEventArgs e)
		{
			Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
			{
				ValueBox.Text = e.Temperature.ToString();
			}));
		}

		private void sensor_Attach(object sender, AttachEventArgs e)
		{
			SetAttached();
			CheckCanSave();
		}

		private void sensor_Detach(object sender, DetachEventArgs e)
		{
			SetAttached();
			CheckCanSave();
		}

		private void SetAttached()
		{
			Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
			{
				if (sensor.Attached)
				{
					phidgetAttached.Visibility = Visibility.Collapsed;
				}
				else
				{
					phidgetAttached.Visibility = Visibility.Visible;
				}
			}));
		}

		public override void OnSave()
		{
			Increasing = (bool)IncreasingRadio.IsChecked;
		}

		private void TextChanged(object sender, TextChangedEventArgs e)
		{
			CheckCanSave();
		}

		public override void OnClosing()
		{
			sensor.Attach -= sensor_Attach;
			sensor.Detach -= sensor_Detach;
			sensor.TemperatureChange -= sensor_TemperatureChange;
			PhidgetManager.Release(ref sensor);
		}

		private void CheckCanSave()
		{
			Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
			{
				string error;

				double topValue;

				if (!(double.TryParse(textBoxTopValue.Text, out topValue) && (topValue >= 0 && topValue <= 100)))
				{
					error = "Invalid Top Value";
				}
				else
				{
					error = string.Empty;
				}

				textInvalid.Text = error;
				if (error == string.Empty)
				{
					textInvalid.Visibility = Visibility.Collapsed;
				}
				else
				{
					textInvalid.Visibility = Visibility.Visible;
				}

				TopValue = topValue;

				CanSave = (sensor.Attached && error == string.Empty);
			}));
		}

		public override string Title
		{
			get
			{
				return "Phidget: Temperature";
			}
		}
	}
}
