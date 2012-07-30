using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using GoogleModules.Resources;
using MayhemCore;
using MayhemWpf.UserControls;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// User Control for setting the information about the moment that needs to be added.
    /// </summary>
    public partial class GooglePlusAddMomentConfig : WpfConfiguration
    {
        public string MomentText
        {
            get;
            private set;
        }

        public string RefreshToken
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        private static AutoResetEvent eventAuthorizationCodeEnter = new AutoResetEvent(false);
        private static AutoResetEvent eventWaitAuthorization = new AutoResetEvent(false);

        private const string Scope = "https://www.googleapis.com/auth/plus.moments.write";
        private string authorizationCode;
        private OAuth2Authenticator<NativeApplicationClient> auth;

        private Thread loadTokenThread;

        private bool isAuthenticated;
        private bool canEnableCheckCode;
        private bool authenticationFailed;

        public GooglePlusAddMomentConfig(string momentText, string title, string momentDetailsText, string momentTypeText)
        {
            InitializeComponent();

            isAuthenticated = false;
            MomentText = momentText;
            configTitle = title;
            DetailsText.Text = momentDetailsText;
            TypeText.Text = momentTypeText;

            CheckValidity();
        }

        public override void OnLoad()
        {
            CanSave = true;
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
            string errorString = string.Empty;
            CanSave = true;

            errorString = CheckValidityMomentText();

            if (errorString.Equals(string.Empty))
            {
                errorString = CheckValidityAuthorizationCode();
            }

            if (errorString.Equals(string.Empty) && authenticationFailed)
            {
                errorString = Strings.GooglePlus_AuthenticationFailed;
            }

            if (errorString.Equals(string.Empty) && !isAuthenticated)
            {
                errorString = Strings.GooglePlus_NotAuthenticated;
            }

            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                CanSave = false;
            }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private string CheckValidityMomentText()
        {
            int textLength = ActivityTextBox.Text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
            {
                errorString = Strings.GooglePlus_MomentText_NoCharacter;
            }
            else if (textLength > 300)
            {
                errorString = Strings.GooglePlus_MomentText_TooLong;
            }

            CanSave = textLength > 0 && (textLength <= 300);

            return errorString;
        }

        private string CheckValidityAuthorizationCode()
        {
            int textLength = AuthorizationCodeBox.Text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
            {
                errorString = Strings.GooglePlus_AuthorizationCode_NoCharacter;
            }
            else if (textLength > 300)
            {
                errorString = Strings.GooglePlus_AuthorizationCode_TooLong;
            }

            CanSave = textLength > 0 && (textLength <= 200);

            // If an Authorization Code is setted and the Authentication process is started we can click the CheckCode button.
            if (CanSave && canEnableCheckCode)
            {
                buttonCheckCode.IsEnabled = true;
            }
            else
            {
                buttonCheckCode.IsEnabled = false;
            }

            return errorString;
        }

        private void AuthorizationCodeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        private void ActivityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        protected void Authenticate()
        {
            NativeApplicationClient provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = Strings.GooglePlus_ClientID;
            provider.ClientSecret = Strings.GooglePlus_ClientSecret;

            auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);

            loadTokenThread = new Thread(new ThreadStart(auth.LoadAccessToken));
            loadTokenThread.Start();
        }

        protected IAuthorizationState GetAuthorization(NativeApplicationClient arg)
        {
            authenticationFailed = false;
            isAuthenticated = false;

            IAuthorizationState state = null;

            canEnableCheckCode = true;

            try
            {
                // Get the auth URL.
                state = new AuthorizationState(new[] { Scope });
                state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
                Uri authUri = arg.RequestUserAuthorization(state);

                // Request authorization from the user (by opening a browser window).
                Process.Start(authUri.ToString());

                eventAuthorizationCodeEnter.WaitOne();

                // Retrieve the access token by using the authorization code.
                return arg.ProcessUserAuthorization(authorizationCode, state);
            }
            catch (Exception ex)
            {
                authenticationFailed = true;

                Logger.Write(ex);

                return null;
            }
            finally
            {
                if (authenticationFailed)
                {
                    isAuthenticated = false;
                }
                else
                {
                    isAuthenticated = true;

                    if (state != null)
                    {
                        RefreshToken = state.RefreshToken;
                    }
                }

                // The user needs to authenticate again in order to get another code.
                canEnableCheckCode = false;
                eventWaitAuthorization.Set();
            }
        }

        private void buttonAuthenticate_Click(object sender, RoutedEventArgs e)
        {
            Authenticate();
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
