using System;
using System.Globalization;
using System.Runtime.Serialization;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.GData.Client;
using Google.GData.Extensions.MediaRss;
using Google.GData.YouTube;
using Google.YouTube;
using GoogleModules.Resources;
using GoogleModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace GoogleModules.Reactions
{
    /// <summary>
    /// A class used for uploading a video to an YouYube chanel.
    /// </summary>
    [DataContract]
    [MayhemModule("YouTube: Upload Video", "Uploads a video to the predefined user's YouTube channel")]
    public class YouTubeUploadVideo : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string videoTitle;

        [DataMember]
        private string description;

        [DataMember]
        private string category;

        [DataMember]
        private string videoPath;

        [DataMember]
        private string refreshToken;

        private const string Scope = "https://gdata.youtube.com";

        private YouTubeRequest request;

        public override void Perform()
        {
            try
            {
                UseSavedAuthorization();

                Video newVideo = new Video();
                newVideo.Title = videoTitle;
                newVideo.Tags.Add(new MediaCategory("Autos", YouTubeNameTable.CategorySchema));
                newVideo.Description = description;
                newVideo.YouTubeEntry.Private = false;
                newVideo.YouTubeEntry.MediaSource = new MediaFileSource(videoPath, "video/mp4");

                Video createdVideo = request.Upload(newVideo);

                ErrorLog.AddError(ErrorType.Message, string.Format(Strings.YouTube_UploadSuccesful, videoTitle));
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.YouTube_UploadFailed, videoTitle));
                Logger.Write(ex);
            }
        }

        public void UseSavedAuthorization()
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = Strings.Google_ClientID;
            provider.ClientSecret = Strings.Google_ClientSecret;

            OAuth2Authenticator<NativeApplicationClient> auth = new OAuth2Authenticator<NativeApplicationClient>(provider, getState);

            auth.LoadAccessToken();
        }

        public IAuthorizationState getState(NativeApplicationClient arg)
        {
            IAuthorizationState state = new AuthorizationState(new[] { Scope });
            state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
            state.RefreshToken = refreshToken;

            arg.RefreshToken(state);

            YouTubeRequestSettings settings = new YouTubeRequestSettings(Strings.YouTube_ProductName, Strings.YouTube_DeveloperKey, state.AccessToken);
            request = new YouTubeRequest(settings);

            return state;
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new YouTubeUploadVideoConfig(videoTitle, description, category, videoPath, Strings.YouTubeUploadVideo_Title); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as YouTubeUploadVideoConfig;

            videoTitle = config.VideoTitle;
            description = config.Description;
            category = config.Category;
            videoPath = config.VideoPath;
            refreshToken = config.RefreshToken;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.UploadVideo_ConfigString, videoTitle);
        }

        #endregion
    }
}
