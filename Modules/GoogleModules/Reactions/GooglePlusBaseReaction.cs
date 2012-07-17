using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Plus.v1moments;
using Google.Apis.Plus.v1moments.Data;
using GoogleModules.Resources;
using GoogleModules.Wpf;
using MayhemCore;
using MayhemWpf.UserControls;

namespace GoogleModules.Reactions
{
    [DataContract]
    public abstract class GooglePlusBaseReaction : ReactionBase
    {
        [DataMember]
        protected string MomentUrl;

        protected DateTime lastAddedItemTimestamp;

        protected const string Scope = "https://www.googleapis.com/auth/plus.moments.write";
        protected PlusService service;

        protected void Authentificate()
        {
            NativeApplicationClient provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = Strings.GooglePlus_ClientID;
            provider.ClientSecret = Strings.GooglePlus_ClientSecret;

            OAuth2Authenticator<NativeApplicationClient> auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);
            service = new PlusService(auth);

            auth.LoadAccessToken();
        }

        protected void AddActivity(string activityType)
        {
            Moment moment = new Moment();
            ItemScope itemScope = new ItemScope();

            itemScope.Url = MomentUrl;

            moment.Type = activityType;           
            moment.Target = itemScope;

            MomentsResource.InsertRequest insReq = service.Moments.Insert(moment, "me", MomentsResource.Collection.Vault);

            Moment mom = insReq.Fetch();
        }

        protected static IAuthorizationState GetAuthorization(NativeApplicationClient arg)
        {
            try
            {
                // Get the auth URL:
                IAuthorizationState state = new AuthorizationState(new[] { Scope });
                state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
                Uri authUri = arg.RequestUserAuthorization(state);

                // Request authorization from the user (by opening a browser window):
                Process.Start(authUri.ToString());

                InsertKeyWindow keyWindow = new InsertKeyWindow();
                keyWindow.ShowDialog();

                string authCode = keyWindow.AuthorizationCode;

                // Retrieve the access token by using the authorization code:*/
                return arg.ProcessUserAuthorization(authCode, state);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                ErrorLog.AddError(ErrorType.Failure, Strings.GooglePlus_AuthentificationFailed);

                return null;
            }
        }
               
        public abstract override void Perform();

        #region IWpfConfigurable Methods

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as GooglePlusAddMomentConfig;

            if (config == null)
            {
                return;
            }

            MomentUrl = config.MomentText;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.MomentUrl_ConfigString, MomentUrl);
        }

        #endregion
    }
}
