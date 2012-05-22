using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace SocketModules.Wpf
{
	public partial class SocketEventConfig : WpfConfiguration
	{
		public string Phrase
		{
			get;
			private set;
		}

		public SocketEventConfig(string phrase)
		{
			Phrase = phrase;
			InitializeComponent();
		}

		public override string Title
		{
			get
			{
				return "Socket Event";
			}
		}

		public override void OnLoad()
		{
			PhraseTextBox.Text = Phrase;
		}

		public override void OnSave()
		{
			Phrase = PhraseTextBox.Text.Trim();
		}

		private void CheckValidity()
		{
			CanSave = PhraseTextBox.Text.Trim().Length > 0;
		}

		private void PhraseText_TextChanged(object sender, TextChangedEventArgs e)
		{
			CheckValidity();

			textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
		}
	}
}
