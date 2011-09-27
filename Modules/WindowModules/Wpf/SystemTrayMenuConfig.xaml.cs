using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for DebugMessageConfig.xaml
    /// </summary>
    public partial class SystemTrayMenuConfig : WpfConfiguration
    {
        public string Text
        { 
            get; 
            private set;
        }

        public SystemTrayMenuConfig(string message)
        {
            this.Text = message;
            InitializeComponent();
        }

        public override string Title
        {
            get { return "System Tray Menu"; }
        }

        public override void OnLoad()
        {
            MessageBox.Text = this.Text;
        }

        private void CheckValidity()
        {
            CanSave = MessageBox.Text.Trim().Length > 0;
        }

        public override void OnSave()
        {
            Text = MessageBox.Text.Trim();
        }

        private void MessageText_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
