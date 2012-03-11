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

		private InterfaceKit IfKit;

		public Func<int, string> Convertor { get; private set; }

		public PhidgetConfigControl Sensor { get; private set; }

		private bool shouldCheckValidity;

		private bool isValid;

		private bool isAttached;

		public SensorConfig(int index, Func<int, string> conversion, PhidgetConfigControl control)
		{
			Index = index;
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
			IfKit = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: false);

			SensorDataBox.Index = Index;
			SensorDataBox.Convertor = Convertor;

			Sensor.OnLoad();
			Sensor.OnRevalidate += Revalidate;

			sensorControl.Content = Sensor;

			IfKit.Attach += ifKit_Attach;

			// If we have detected sensors already, then we are attached but haven't triggered
			// attached event
			if (IfKit.sensors.Count > 0)
				isAttached = true;

			Revalidate();

			shouldCheckValidity = true;

			CheckCanSave();
		}

		public override void OnSave()
		{
			Index = SensorDataBox.Index;
			Sensor.OnSave();
		}

		public override void OnClosing()
		{
			IfKit.Attach -= ifKit_Attach;

			PhidgetManager.Release<InterfaceKit>(ref IfKit);
		}

		private void SetAttachedMessage(bool attached)
		{
			Visibility visible = Visibility.Visible;
			if (attached)
				visible = Visibility.Collapsed;

			Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
			{
				phidgetAttached.Visibility = visible;
			}));
		}

		private void CheckCanSave()
		{
			CanSave = isAttached && isValid;

			SetAttachedMessage(isAttached);
		}

		private void ifKit_Attach(object sender, AttachEventArgs e)
		{
			isAttached = true;

			CheckCanSave();
		}

		private void Revalidate()
		{
			string text = Sensor.GetErrorString();
			isValid = string.IsNullOrEmpty(text);

			if (shouldCheckValidity)
			{
				textInvalid.Text = text;
				textInvalid.Visibility = string.IsNullOrEmpty(text) ? Visibility.Collapsed : Visibility.Visible;

				CheckCanSave();
			}
		}
	}
}
