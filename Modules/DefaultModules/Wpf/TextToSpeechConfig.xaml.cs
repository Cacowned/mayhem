using System.Windows;
using MayhemDefaultStyles.UserControls;

namespace DefaultModules.Wpf
{
    /// <summary>
    /// Interaction logic for TextToSpeechConfig.xaml
    /// </summary>
    public partial class TextToSpeechConfig : IWpfConfiguration
    {
        public string Message;

        public TextToSpeechConfig(string message)
        {
            this.Message = message;
            InitializeComponent();

            MessageText.Text = this.Message;
            CanSave = message.Length > 0;
            textInvalid.Visibility = Visibility.Collapsed;
        }

        public override string Title
        {
            get { return "Text To Speech"; }
        }

        public override bool OnSave()
        {
            if (MessageText.Text.Trim().Length == 0)
            {
                MessageBox.Show("You must provide a message");
                return false;
            }
            Message = MessageText.Text.Trim();
            return true;
        }

        public override void OnCancel()
        {
        }

        private void MessageText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CanSave = MessageText.Text.Length > 0;
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
