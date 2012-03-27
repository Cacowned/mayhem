using System;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MayhemSerial;

namespace SerialModules.Wpf
{
	/// <summary>
	/// Interaction logic for PortSelector.xaml
	/// </summary>
	public partial class PortSelector : UserControl
	{
		public event EventHandler CanSaveChanged;
		private bool canSave;
		public bool CanSave
		{
			get
			{
				return this.canSave;
			}
			private set
			{
				this.canSave = value;
				if (this.CanSaveChanged != null)
				{
					this.CanSaveChanged(this, new EventArgs());
				}
			}
		}

		public string PortName
		{
			get;
			set;
		}

		public SerialSettings Settings
		{
			get;
			set;
		}

		public PortSelector()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.RefreshPorts();

			BaudBox.Text = this.Settings.BaudRate.ToString();

			ParityBox.ItemsSource = Enum.GetNames(typeof(Parity));
			ParityBox.SelectedValue = Enum.GetName(typeof(Parity), this.Settings.Parity);

			StopBitsBox.ItemsSource = Enum.GetNames(typeof(StopBits));
			StopBitsBox.SelectedValue = Enum.GetName(typeof(StopBits), this.Settings.StopBits);

			DataBitsBox.Text = this.Settings.DataBits.ToString();
		}

		public void OnSave()
		{
			this.PortName = PortBox.SelectedValue.ToString();

			// Set the settings (validate baud rate and databits as you go)

			// This should work because we validated already
			int baud = int.Parse(BaudBox.Text);
			Parity parity = (Parity)Enum.Parse(typeof(Parity), ParityBox.SelectedValue.ToString());
			StopBits stopBits = (StopBits)Enum.Parse(typeof(StopBits), StopBitsBox.SelectedValue.ToString());
			int dataBits = int.Parse(DataBitsBox.Text);

			this.Settings = new SerialSettings(baud, parity, stopBits, dataBits);
		}

		private void RefreshPorts()
		{
			PortBox.ItemsSource = SerialPortManager.AllPorts;

			if (SerialPortManager.AllPorts.Contains(this.PortName))
			{
				PortBox.SelectedValue = this.PortName;
			}
			else
			{
				PortBox.SelectedIndex = 0;
			}
		}

		private void RefreshPorts_Click(object sender, RoutedEventArgs e)
		{
			this.RefreshPorts();
		}

		private void BaudBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdateCanSave();
		}

		private void DataBitsBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdateCanSave();
		}

		private void UpdateCanSave()
		{
			// To be able to save, Baud Rate and Data Bits have to be valid
			// info
			bool tempCanSave = false;

			int baud = 0;
			if (!int.TryParse(BaudBox.Text, out baud))
			{
				invalidRate.Visibility = Visibility.Visible;
				tempCanSave = false;
			}
			else
			{
				invalidRate.Visibility = Visibility.Collapsed;
				tempCanSave = true;
			}

			int dataBits = 0;
			if (!int.TryParse(DataBitsBox.Text, out dataBits))
			{
				invalidBits.Visibility = Visibility.Visible;
				tempCanSave = false;
			}
			else
			{
				invalidBits.Visibility = Visibility.Collapsed;
				tempCanSave = true && tempCanSave;
			}

			this.CanSave = tempCanSave;
		}
	}
}
