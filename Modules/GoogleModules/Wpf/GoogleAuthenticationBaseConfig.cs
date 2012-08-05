using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Controls;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using GoogleModules.Resources;
using MayhemCore;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// This is the base class for the configuration windows that use the authentication process.
    /// </summary>
    public class GoogleAuthenticationBaseConfig : GoogleBaseConfig
    {
        public string RefreshToken
        {
            get;
            private set;
        }

        protected bool isAuthenticated;
        protected bool canEnableCheckCode;
        protected bool authenticationFailed;

        protected string authorizationCode;
        protected OAuth2Authenticator<NativeApplicationClient> auth;

        protected Thread loadTokenThread;

        protected static AutoResetEvent eventAuthorizationCodeEnter = new AutoResetEvent(false);
        protected static AutoResetEvent eventWaitAuthorization = new AutoResetEvent(false);

        private string ReactionScope;

        protected bool CheckValidityAuthorizationCode(string authorizationCode, Button buttonCheckCode)
        {
            errorString = string.Empty;

            if (!CheckValidityField(authorizationCode, 200, Strings.General_AuthorizationCode))
            {
                buttonCheckCode.IsEnabled = false;
                return false;
            }
            else
            {
                buttonCheckCode.IsEnabled = canEnableCheckCode;
                return true;
            }
        }

        protected bool CheckAuthentication()
        {
            errorString = string.Empty;

            if (authenticationFailed)
            {
                errorString = Strings.General_AuthenticationFailed;
                return false;
            }

            if (!isAuthenticated)
            {
                errorString = Strings.General_NotAuthenticated;
                return false;
            }

            return true;
        }

        protected void Authenticate(string scope)
        {
            ReactionScope = scope;

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
                state = new AuthorizationState(new[] { ReactionScope });
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
    }
}
