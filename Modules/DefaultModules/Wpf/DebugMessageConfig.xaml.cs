using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace DefaultModules.Wpf
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

        private void MessageText_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
