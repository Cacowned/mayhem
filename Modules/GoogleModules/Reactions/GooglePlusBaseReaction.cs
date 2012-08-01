using System;
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
    /// <summary>
    /// An abstract base class used for posting information to the Google+ History page.
    /// </summary>
    [DataContract]
    public abstract class GooglePlusBaseReaction : ReactionBase
    {
        /// <summary>
        /// An Url that describes the type of the moment the user will add to the Google+ History page.
        /// </summary>
        [DataMember]
        protected string momentUrl;

        [DataMember]
        protected string refreshToken;

        protected DateTime lastAddedItemTimestamp;

        protected PlusService service;
        protected OAuth2Authenticator<NativeApplicationClient> auth;
        protected const string Scope = "https://www.googleapis.com/auth/plus.moments.write";

        protected void AddActivity(string activityType)
        {
            Moment moment = new Moment();
            ItemScope itemScope = new ItemScope();

            UseSavedAuthorization();

            itemScope.Url = momentUrl;

            moment.Type = activityType;
            moment.Target = itemScope;

            // Preparing and making the request.
            service.Moments.Insert(moment, "me", MomentsResource.Collection.Vault).Fetch();
        }

        public void UseSavedAuthorization()
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = Strings.Google_ClientID;
            provider.ClientSecret = Strings.Google_ClientSecret;

            OAuth2Authenticator<NativeApplicationClient> auth = new OAuth2Authenticator<NativeApplicationClient>(provider, getState);

            service = new PlusService(auth);

            auth.LoadAccessToken();
        }

        public IAuthorizationState getState(NativeApplicationClient arg)
        {
            IAuthorizationState state = new AuthorizationState(new[] { Scope });
            state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
            state.RefreshToken = refreshToken;

            arg.RefreshToken(state);

            return state;
        }

        public abstract override void Perform();

        #region IWpfConfigurable Methods

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as GooglePlusAddMomentConfig;

            momentUrl = config.MomentText;
            refreshToken = config.RefreshToken;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.MomentUrl_ConfigString, momentUrl);
        }

        #endregion
    }
}
