using System.Windows;
using MayhemWpf.UserControls;

namespace DefaultModules.Wpf
{
    public partial class TextToSpeechConfig : WpfConfiguration
    {
        public string Message
        {
            get;
            private set;
        }

        public TextToSpeechConfig(string message)
        {
            this.Message = message;

            InitializeComponent();
        }

        public override string Title
        {
            get { return "Text To Speech"; }
        }

        public override void OnLoad()
        {
            MessageBox.Text = this.Message;
        }

        public override void OnSave()
        {
            Message = MessageBox.Text.Trim();
        }

        private void CheckValidity()
        {
            CanSave = MessageBox.Text.Trim().Length > 0;
        }

        private void MessageText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CheckValidity();

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
