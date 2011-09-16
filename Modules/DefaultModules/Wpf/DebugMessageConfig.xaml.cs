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

        private bool shouldCheckValidity = false;

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
            MessageText.Text = this.Message;

            shouldCheckValidity = true;
        }

        private void CheckValidity()
        {
            Message = MessageText.Text.Trim();
            CanSave = Message.Length > 0;
        }

        private void MessageText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (shouldCheckValidity)
            {
                CheckValidity();

                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
