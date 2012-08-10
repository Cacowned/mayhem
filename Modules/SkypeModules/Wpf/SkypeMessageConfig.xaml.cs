using System.Windows;
using SkypeModules.Resources;

namespace SkypeModules.Wpf
{
    /// <summary>
    /// User Control for setting the message that will be send to user with the selected Skype ID.
    /// </summary>
    public partial class SkypeMessageConfig : SkypeBaseConfig
    {
        public string Message
        {
            get;
            private set;
        }

        public SkypeMessageConfig(string skypeID, string message, string title)
        {
            SkypeID = skypeID;
            Message = message;
            configTitle = title;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            SkypeIDBox.Text = SkypeID;
            MessageBox.Text = Message;

            CheckValidity();
        }

        public override void OnSave()
        {
            SkypeID = SkypeIDBox.Text;
            Message = MessageBox.Text;
        }

        protected override void CheckValidity()
        {
            errorString = string.Empty;

            // The evaluation variable is not used but it won't compile if I don't store the result.
            bool evaluation = CheckValidityField(SkypeIDBox.Text, Strings.SkypeID, maxLength: 100) &&
                              CheckValidityField(MessageBox.Text, Strings.Message, maxLength: 100);

            DisplayErrorMessage(textInvalid);
        }
    }
}
