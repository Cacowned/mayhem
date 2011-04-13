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

namespace OfficeModules.Wpf
{
	/// <summary>
	/// Interaction logic for SmsMessageConfig.xaml
	/// </summary>
	public partial class SmsMessageConfig : Window
	{
		public string to;
		public string msg;

		public string carrierString;

		protected Dictionary<string, string> carriers;

		// TODO: This is probably a security hole
		public string password;

		public SmsMessageConfig(string to, string message, Dictionary<string, string> carriers) {
			this.to = to;
			this.msg = message;
			this.carriers = carriers;

			InitializeComponent();
		}

		protected override void OnInitialized(System.EventArgs e) {
			base.OnInitialized(e);

			ToBox.Text = to;
			MsgBox.Text = msg;
			Carrier.ItemsSource = this.carriers.Keys;

			Carrier.SelectedIndex = 0;
		}

		private void Button_Save_Click(object sender, RoutedEventArgs e) {

			to = ToBox.Text;
			msg = MsgBox.Text;
			carrierString = carriers[Carrier.SelectedValue.ToString()];

			DialogResult = true;
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}
	}
}
