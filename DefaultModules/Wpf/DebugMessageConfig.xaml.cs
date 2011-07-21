using System.Windows;

namespace DefaultModules.Wpf
{
	/// <summary>
	/// Interaction logic for DebugMessageConfig.xaml
	/// </summary>
	public partial class DebugMessageConfig : Window
	{
		public string message;

		public DebugMessageConfig(string message) {
			this.message = message;
			InitializeComponent();

			MessageText.Text = this.message;
		}

		private void Button_Save_Click(object sender, RoutedEventArgs e) {
			if (MessageText.Text.Trim().Length == 0) {
				MessageBox.Show("You must provide a message");
				return;
			}
			message = MessageText.Text.Trim();
			DialogResult = true;
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}
	}
}
