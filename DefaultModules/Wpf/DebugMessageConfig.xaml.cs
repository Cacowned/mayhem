using System.Windows;
using System.Windows.Controls;
using MayhemDefaultStyles.UserControls;

namespace DefaultModules.Wpf
{
    /// <summary>
    /// Interaction logic for DebugMessageConfig.xaml
    /// </summary>
    public partial class DebugMessageConfig : IWpfConfiguration
    {
        public string Message;

        public DebugMessageConfig(string message)
        {
            this.Message = message;
            InitializeComponent();

            MessageText.Text = this.Message;
            CanSave = message.Length > 0;
            textInvalid.Visibility = Visibility.Collapsed;
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

        public override string Title
        {
            get { return "Debug Message"; }
        }

        private void MessageText_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanSave = MessageText.Text.Length > 0;
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
