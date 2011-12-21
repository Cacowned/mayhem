using System;
using System.Windows;
using System.Windows.Threading;
using MayhemWpf.UserControls;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Wpf.UserControls
{
	/// <summary>
	/// Interaction logic for SensorConfig.xaml
	/// </summary>
	public partial class SensorConfig : WpfConfiguration
	{
		public int Index { get; private set; }

		public InterfaceKit IfKit { get; private set; }

		public Func<int, string> Convertor { get; private set; }

		public PhidgetConfigControl Sensor { get; private set; }

		private bool shouldCheckValidity;

		public SensorConfig(InterfaceKit ifKit, int index, Func<int, string> conversion, PhidgetConfigControl control)
		{
			Index = index;
			IfKit = ifKit;
			Convertor = conversion;

			Sensor = control;

			InitializeComponent();
		}

		public override string Title
		{
			get
			{
				return Sensor.Title;
			}
		}

		public override void OnLoad()
		{
			SensorDataBox.Index = Index;
			SensorDataBox.IfKit = IfKit;
			SensorDataBox.Convertor = Convertor;
			SensorDataBox.Setup();

			Sensor.OnLoad();
			Sensor.OnRevalidate += Revalidate;

			sensorControl.Content = Sensor;

			IfKit.Attach += ifKit_Attach;

			CanSavedChanged += PhidgetConfig_CanSavedChanged;

			// If we have detected sensors already, then enable the save button
			if (IfKit.sensors.Count > 0)
				CanSave = true;

			shouldCheckValidity = true;
		}

		public override void OnSave()
		{
			Index = SensorDataBox.Index;
			Sensor.OnSave();
		}

		public override void OnClosing()
		{
			IfKit.Attach -= ifKit_Attach;
			CanSavedChanged -= PhidgetConfig_CanSavedChanged;
		}

		private void PhidgetConfig_CanSavedChanged(bool canSave)
		{
			Visibility visible = Visibility.Visible;
			if (canSave)
				visible = Visibility.Collapsed;

			Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
			{
				phidgetAttached.Visibility = visible;
			}));
		}

		private void ifKit_Attach(object sender, AttachEventArgs e)
		{
			CanSave = true;
		}

		private void Revalidate()
		{
			if (shouldCheckValidity)
			{
				string text = Sensor.CheckValidity();
				textInvalid.Text = text;
				textInvalid.Visibility = text.Length == 0 ? Visibility.Collapsed : Visibility.Visible;
			}
		}
	}
}
