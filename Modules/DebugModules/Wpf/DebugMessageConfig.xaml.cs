using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace DebugModules.Wpf
{
    /// <summary>
    /// Interaction logic for DebugMessageConfig.xaml
    /// </summary>
    public partial class DebugMessageConfig : WpfConfiguration
    {
        public string Message
        {
            get;
            private set;
        }

        public DebugMessageConfig(string message)
        {
            this.Message = message;
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Debug Message"; }
        }

        public override void OnLoad()
        {
            MessageTextBox.Text = this.Message;
        }

        public override void OnSave()
        {
            Message = MessageTextBox.Text.Trim();
        }

        private void CheckValidity()
        {
            CanSave = MessageTextBox.Text.Trim().Length > 0;
        }

        private void MessageText_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
