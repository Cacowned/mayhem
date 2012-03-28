using System;
using System.Windows;
using System.Windows.Controls;
using MayhemSerial;
using MayhemWpf.UserControls;

namespace SerialModules.Wpf
{
	/// <summary>
	/// Interaction logic for SerialListenConfig.xaml
	/// </summary>
	public partial class SerialWriteConfig : WpfConfiguration
	{
		public string Phrase
		{
			get;
			private set;
		}

		public SerialWriteConfig(string portName, SerialSettings settings, string phrase)
		{
			InitializeComponent();

			Selector.PortName = portName;
			Selector.Settings = settings;

			Selector.CanSaveChanged += new EventHandler(this.Selector_CanSaveChanged);

			this.Phrase = phrase;
		}

		public override void OnLoad()
		{
			PhraseBox.Text = this.Phrase;
		}

		public override void OnSave()
		{
			this.Phrase = PhraseBox.Text;
			Selector.OnSave();
		}

		public override string Title
		{
			get
			{
				return "Serial Write";
			}
		}

		private void PhraseBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdateCanSave();
		}

		private void Selector_CanSaveChanged(object sender, System.EventArgs e)
		{
			this.UpdateCanSave();
		}

		private void UpdateCanSave()
		{
			// NullOrEmpty instead of NullOrWhitespace because someone could want to listen for a space as a special
			// character.
			if (string.IsNullOrEmpty(PhraseBox.Text))
			{
				invalidPhrase.Visibility = Visibility.Visible;
				CanSave = false;
			}
			else
			{
				invalidPhrase.Visibility = Visibility.Collapsed;

				// We can save, so we can save overall if the selector can save too
				CanSave = Selector.CanSave;
			}
		}
	}
}
