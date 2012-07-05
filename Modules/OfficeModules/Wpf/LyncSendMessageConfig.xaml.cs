using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using OfficeModules.Resources;

namespace OfficeModules.Wpf
{
    /// <summary>
    /// User Control for sending a message to a predefined user.
    /// </summary>
    public partial class LyncSendMessageConfig : WpfConfiguration
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public string UserId
        {
            get;
            private set;
        }

        /// <summary>
        /// The message that needs to be sent.
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// The title of the user control.
        /// </summary>
        public override string Title
        {
            get { return "Lync: Send Instant Message"; }
        }

        /// <summary>
        /// The constructor of the LyncSendMessageConfig class.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="message">The message</param>
        public LyncSendMessageConfig(string userId, string message)
        {
            UserId = userId;
            Message = message;

            InitializeComponent();
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
        public override void OnLoad()
        {
            UserIdBox.Text = UserId;
            MessageTextBox.Text = Message;

            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the user clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            UserId = UserIdBox.Text;
            Message = MessageTextBox.Text;
        }

        /// <summary>
        /// This method will check the validity of the information provided by the user.
        /// </summary>
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

        /// <summary>
        /// This method will be called when the text from the UserIdBox changes.
        /// </summary>
        private void UserIdBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the text from the MessageTextBox changes.
        /// </summary>
        private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
