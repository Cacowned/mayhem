using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using System.IO;
using System;

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

			updateCommand();
		}

		private void updateCommand()
		{
			string directory = Directory.GetCurrentDirectory();
			string filename = "SocketExecutable.exe";
			string commandText = string.Format("{0}\\{1} \"{2}\"", directory, filename, PhraseTextBox.Text);
			
			command.Text = commandText;
			command.PageRight();
		}

		private void Copy_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(command.Text);
		}
	}
}
