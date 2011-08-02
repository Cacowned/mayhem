using System.Windows;

namespace DefaultModules.Wpf
{
	/// <summary>
	/// Interaction logic for TextToSpeechConfig.xaml
	/// </summary>
	public partial class TextToSpeechConfig : Window
	{
		public string message;

		public TextToSpeechConfig(string message) {
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
