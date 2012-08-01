using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Google.GData.Client;
using Google.YouTube;
using GoogleModules.Resources;
using GoogleModules.Wpf;
using MayhemCore;
using MayhemWpf.UserControls;

namespace GoogleModules.Events
{
    /// <summary>
    /// This is the base class for the Youtube Video Events.
    /// </summary>
    [DataContract]
    public abstract class YouTubeVideoEventBase : YouTubeEventBase
    {
        [DataMember]
        protected string username;

        protected Uri feedUri;
        protected Feed<Video> videoFeed;
        protected List<Video> currentVideoList;

        protected DateTime lastVideoAddedTimestamp;

        protected override void OnEnabling(EnablingEventArgs e)
        {
            lastVideoAddedTimestamp = DateTime.MinValue;

            try
            {
                InitializeYoutubeConnection();

                feedUri = new Uri("http://gdata.youtube.com/feeds/api/users/" + username + "/uploads");
                videoFeed = request.Get<Video>(feedUri);

                currentVideoList = videoFeed.Entries.ToList();
            }
            catch (GDataRequestException ex)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.General_Incorrect, Strings.General_ProfileID));
                Logger.Write(ex);

                e.Cancel = true;
                return;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.GooglePlus_ErrorMonitoringYoutubeUploadedVideos, username));
                Logger.Write(ex);

                e.Cancel = true;
                return;
            }
        }

        #region IWpfConfigurable Methods

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as YouTubeUsernameConfig;

            username = config.Username;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.Username_ConfigString, username);
        }

        #endregion
    }
}
