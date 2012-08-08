using System.Windows;
using System.Windows.Controls;
using GoogleModules.Resources;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// User Control for setting the information about the moment that needs to be added.
    /// </summary>
    public partial class GooglePlusAddMomentConfig : GoogleAuthenticationBaseConfig
    {
        public string MomentText
        {
            get;
            private set;
        }

        private const string Scope = "https://www.googleapis.com/auth/plus.moments.write";

        public GooglePlusAddMomentConfig(string momentText, string title, string momentDetailsText, string momentTypeText)
        {
            InitializeComponent();

            isAuthenticated = false;
            MomentText = momentText;
            configTitle = title;
            DetailsText.Text = momentDetailsText;
            TypeText.Text = momentTypeText;
        }

        public override void OnLoad()
        {
            buttonCheckCode.IsEnabled = false;
            canEnableCheckCode = false;

            ActivityTextBox.Text = MomentText;

            CheckValidity();
        }

        public override void OnSave()
        {
            MomentText = ActivityTextBox.Text;
        }

        public override void OnCancel()
        {
            // If loadTokenThread is started we need to stop it.
            if (loadTokenThread != null && loadTokenThread.IsAlive)
            {
                loadTokenThread.Abort();
            }
        }

        private void CheckValidity()
        {
            errorString = string.Empty;

            // The text fields of the configuration window are checked in order and if an error is found the evaluation of this expresion will stop and the error will be displayed.
            // The evaluation variable is not used but it won't compile if I don't store the result.
            bool evaluation = CheckValidityField(ActivityTextBox.Text, Strings.General_MomentText, maxLength: 300) &&
                              CheckValidityField(AuthorizationCodeBox.Text, Strings.General_AuthorizationCode, maxLength: 300) &&
                              CheckValidityAuthorizationCode(AuthorizationCodeBox.Text, buttonCheckCode) &&
                              CheckAuthentication();

            DisplayErrorMessage(textInvalid);
        }

        private void Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        private void buttonAuthenticate_Click(object sender, RoutedEventArgs e)
        {
            Authenticate(Scope);
        }

        private void buttonCheckCode_Click(object sender, RoutedEventArgs e)
        {
            authorizationCode = AuthorizationCodeBox.Text;
            eventAuthorizationCodeEnter.Set();

            // We need to wait for the authentication to take place. If in 15 seconds the authentication doesn't take place we stop it.           
            eventWaitAuthorization.WaitOne(15000);

            CheckValidity();
            buttonCheckCode.IsEnabled = false;
        }
    }
}
