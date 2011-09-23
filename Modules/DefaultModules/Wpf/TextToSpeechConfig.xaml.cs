using System.Windows;
using MayhemWpf.UserControls;

namespace DefaultModules.Wpf
{
    public partial class TextToSpeechConfig : IWpfConfiguration
    {
        public string Message;

        private bool shouldCheckValidity = false;

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

            shouldCheckValidity = true;
        }

        private void CheckValidity()
        {
            Message = MessageBox.Text.Trim();

            CanSave = Message.Length > 0;
        }

        private void MessageText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (shouldCheckValidity)
            {
                CheckValidity();

                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
