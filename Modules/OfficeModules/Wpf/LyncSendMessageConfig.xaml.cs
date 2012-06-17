using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using OfficeModules.Resources;

namespace OfficeModules.Wpf
{
    public partial class LyncSendMessageConfig : WpfConfiguration
    {
        public string UserId
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return "Send message"; }
        }

        public LyncSendMessageConfig(string userId, string message)
        {
            UserId = userId;
            Message = message;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            UserIdBox.Text = UserId;
            MessageTextBox.Text = Message;

            CheckValidity();
        }

        public override void OnSave()
        {
            UserId = UserIdBox.Text;
            Message = MessageTextBox.Text;
        }

        private void CheckValidity()
        {
            CanSave = UserIdBox.Text.Trim().Length > 0;

            if (!CanSave)
            {
                textInvalid.Text = Strings.Lync_InvalidUserId;
            }
            else
            {
                CanSave = MessageTextBox.Text.Trim().Length > 0;
                if (!CanSave)
                {
                    textInvalid.Text = Strings.Lync_MessageInvalid;
                }
            }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void UserIdBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
