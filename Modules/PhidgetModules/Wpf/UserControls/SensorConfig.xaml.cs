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
		
		private InterfaceKitType type;

		public SensorConfig(int index, Func<int, string> conversion, PhidgetConfigControl control, InterfaceKitType type = InterfaceKitType.Sensor)
		{
			Index = index;
			Convertor = conversion;

			Sensor = control;

			this.type = type;

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

			SensorDataBox.InterfaceKitType = type;
			SensorDataBox.Index = Index;
			SensorDataBox.Convertor = Convertor;

			Sensor.OnLoad();
			Sensor.OnRevalidate += Revalidate;

			sensorControl.Content = Sensor;

			IfKit.Attach += ifKit_Attach;
			IfKit.Detach += IfKit_Detach;

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
			Sensor.OnRevalidate -= Revalidate;
			IfKit.Attach -= ifKit_Attach;
			IfKit.Detach -= IfKit_Detach;

			PhidgetManager.Release(ref IfKit);
		}

		private void SetAttachedMessage()
		{
			Visibility visible = Visibility.Visible;
			if (IfKit.Attached)
				visible = Visibility.Collapsed;

			Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
			{
				phidgetAttached.Visibility = visible;
			}));
		}

		private void CheckCanSave()
		{
			CanSave = IfKit.Attached && isValid;

			SetAttachedMessage();
		}

		private void ifKit_Attach(object sender, AttachEventArgs e)
		{
			CheckCanSave();
		}

		private void IfKit_Detach(object sender, DetachEventArgs e)
		{
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
