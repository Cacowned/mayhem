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
    }
}
