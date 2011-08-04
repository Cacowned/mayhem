using System.Windows;
using System.Windows.Controls;
using MayhemDefaultStyles.UserControls;

namespace DefaultModules.Wpf
{
    /// <summary>
    /// Interaction logic for DebugMessageConfig.xaml
    /// </summary>
    public partial class DebugMessageConfig : IWpfConfig
    {
        public string Message;

        public DebugMessageConfig(string message)
        {
            this.Message = message;
            InitializeComponent();

            MessageText.Text = this.Message;
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
    }
}
