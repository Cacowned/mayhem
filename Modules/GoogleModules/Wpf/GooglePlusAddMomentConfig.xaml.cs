using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using GoogleModules.Resources;
using MayhemCore;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// User Control for setting the information about the moment that needs to be added.
    /// </summary>
    public partial class GooglePlusAddMomentConfig : GoogleBaseConfig
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
            errorString = string.Empty;
            CanSave = true;

            if (!CheckValidityField(ActivityTextBox.Text, 300, Strings.General_MomentText))
            {
                DisplayErrorMessage(textInvalid);
                return;
            }

            if (!CheckValidityField(AuthorizationCodeBox.Text, 300, Strings.General_AuthorizationCode))
            {
                DisplayErrorMessage(textInvalid);
                buttonCheckCode.IsEnabled = false;
                return;
            }
            else
            {
                // If the authorization code is valid and the other conditions are satisfied we can enable the Check Code button.
                buttonCheckCode.IsEnabled = canEnableCheckCode;
            }

            if (authenticationFailed)
            {
                errorString = Strings.General_AuthenticationFailed;
                DisplayErrorMessage(textInvalid);
                return;
            }

            if (!isAuthenticated)
            {
                errorString = Strings.General_NotAuthenticated;
                DisplayErrorMessage(textInvalid);
                return;
            }

            // If no error was found we call this method to enable the Save button and hide the error text block.
            DisplayErrorMessage(textInvalid);
        }

        private void Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        protected void Authenticate()
        {
            NativeApplicationClient provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = Strings.Google_ClientID;
            provider.ClientSecret = Strings.Google_ClientSecret;

            auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);

            loadTokenThread = new Thread(new ThreadStart(auth.LoadAccessToken));
            loadTokenThread.Start();
        }

        protected IAuthorizationState GetAuthorization(NativeApplicationClient arg)
        {
            authenticationFailed = false;
            isAuthenticated = false;
            canEnableCheckCode = true;

            IAuthorizationState state = null;

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
