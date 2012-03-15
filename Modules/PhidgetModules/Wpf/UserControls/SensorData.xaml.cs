using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Wpf.UserControls
{
	/// <summary>
	/// Interaction logic for SensorData.xaml
	/// </summary>
	public partial class SensorData : UserControl
	{
		public Func<int, string> Convertor
		{
			get;
			set;
		}

		public int Index
		{
			get;
			set;
		}

		public InterfaceKitType InterfaceKitType
		{
			get;
			set;
		}

		private InterfaceKit IfKit;

		public SensorData()
		{
			InitializeComponent();
		}

		private void IfKit_Attach(object sender, AttachEventArgs e)
		{
			SetUpSensorBox();
		}

		private void SetUpSensorBox()
		{
			Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
			{
				if (InterfaceKitType == InterfaceKitType.Input)
				{
					for (int i = 0; i < IfKit.inputs.Count; i++)
					{
						SensorBox.Items.Add(i);
					}


					if (IfKit.sensors.Count > 0)
					{
						// We want to start with some data.
						// this is kinda a hack. Pass 0 for false,
						// anything else for true
						int value = 0;
						if (IfKit.inputs[Index])
							value = 1;

						SetString(Convertor(value));
					}
				}
				else if (InterfaceKitType == InterfaceKitType.Sensor)
				{
					for (int i = 0; i < IfKit.sensors.Count; i++)
					{
						SensorBox.Items.Add(i);
					}

					if (IfKit.sensors.Count > 0)
					{
						// We want to start with some data.
						SetString(Convertor(IfKit.sensors[Index].Value));
					}
				}

				SensorBox.SelectedIndex = Index;
			}));
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			IfKit = PhidgetManager.Get<InterfaceKit>(false);

			IfKit.Attach += IfKit_Attach;

			if (InterfaceKitType == InterfaceKitType.Input)
				IfKit.InputChange += InputChange;
			else if (InterfaceKitType == InterfaceKitType.Sensor)
				IfKit.SensorChange += SensorChange;

			if (IfKit.Attached)
			{
				SetUpSensorBox();
			}
		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			IfKit.Attach -= IfKit_Attach;

			if (InterfaceKitType == InterfaceKitType.Input)
				IfKit.InputChange -= InputChange;
			else if (InterfaceKitType == InterfaceKitType.Sensor)
				IfKit.SensorChange -= SensorChange;

			PhidgetManager.Release<InterfaceKit>(ref IfKit);
		}

		protected void SensorChange(object sender, SensorChangeEventArgs e)
		{
			Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
			{
				// We only care about the index we are looking at.
				if (e.Index == Index && Convertor != null)
				{
					SetString(Convertor(e.Value));
				}
			}));
		}

		private void InputChange(object sender, InputChangeEventArgs e)
		{
			Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
			{
				// We only care about the index we are looking at.
				if (e.Index == Index && Convertor != null)
				{
					// this is kinda a hack. Pass 0 for false,
					// anything else for true
					int value = 0;
					if (IfKit.inputs[Index])
						value = 1;

					SetString(Convertor(value));
				}
			}));
		}

		protected void SetString(string text)
		{
			Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
			{
				ValueBox.Text = text;
			}));
		}

		private void SensorBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox box = sender as ComboBox;
			Index = box.SelectedIndex;
		}
	}
}
