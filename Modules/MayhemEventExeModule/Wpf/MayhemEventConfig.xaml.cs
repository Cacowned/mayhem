using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using System.IO;
using System;
using System.Threading;

namespace MayhemEventExeModule.Wpf
{
	public partial class MayhemEventConfig : WpfConfiguration
	{
		public string Phrase
		{
			get;
			private set;
		}

		public MayhemEventConfig(string phrase)
		{
			Phrase = phrase;
			InitializeComponent();
		}

		public override string Title
		{
			get
			{
				return "MayhemEvent.exe";
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
			string filename = "MayhemEvent.exe";
			string commandText = string.Format("{0}\\{1} \"{2}\"", directory, filename, PhraseTextBox.Text);
			
			command.Text = commandText;
			command.PageRight();
		}

		private void Copy_Click(object sender, RoutedEventArgs e)
		{
            // TODO: What a super super terrible fix
            // SetText can throw an exception, this stupid thing found on
            // http://stackoverflow.com/questions/68666/clipbrd-e-cant-open-error-when-setting-the-clipboard-from-net
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    Clipboard.SetText(command.Text);
                    return;
                }
                catch { }
                Thread.Sleep(10);
            }  
		}
	}
}
