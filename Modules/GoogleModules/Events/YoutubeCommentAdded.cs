using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Timers;
using Google.GData.Client;
using Google.YouTube;
using GoogleModules.Resources;
using GoogleModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace GoogleModules.Events
{
    /// <summary>
    /// This class represents an event that will be triggered when a comment is received for a predefined video.
    /// </summary>
    [DataContract]
    [MayhemModule("Youtube: Video Comment", "Triggers when a comment is received for a predefined video")]
    public class YoutubeCommentAdded : YoutubeEventBaseClass, IWpfConfigurable
    {
        [DataMember]
        private string videoID;

        private DateTime lastCommentAddedTimestamp;
        private string baseUrl;
        private Video video;

        private bool isFirstTime;

        protected override void OnEnabling(EnablingEventArgs e)
        {
            baseUrl = "http://gdata.youtube.com/feeds/api/videos/";
            lastCommentAddedTimestamp = DateTime.MinValue;

            // We need to check if the video exists.
            try
            {
                InitializeYoutubeConnection();

                Uri videoEntryUrl = new Uri(baseUrl + videoID);
                video = request.Retrieve<Video>(videoEntryUrl);
            }
            catch (GDataRequestException ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.YoutubeVideoDoesntExists);
                Logger.Write(ex);

                e.Cancel = true;
                return;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Youtube_ErrorMonitoringComment);
                Logger.Write(ex);

                e.Cancel = true;
                return;
            }

            isFirstTime = true;

            StartTimer(100);
        }

        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            if (isFirstTime)
            {
                try
                {
                    Feed<Comment> comments = request.GetComments(video);

                    foreach (Comment comment in comments.Entries)
                    {
                        if (lastCommentAddedTimestamp.CompareTo(comment.Updated) < 0)
                        {
                            lastCommentAddedTimestamp = comment.Updated;
                        }
                    }

                    isFirstTime = false;
                }
                catch (Exception ex)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.Youtube_VideoCommentCouldntEnable);
                    Logger.Write(ex);

                    return;
                }
            }
            else
            {
                try
                {
                    Feed<Comment> comments = request.GetComments(video);
                    DateTime newerCommentAddedTimestamp = lastCommentAddedTimestamp;

                    foreach (Comment comment in comments.Entries)
                    {
                        if (lastCommentAddedTimestamp.CompareTo(comment.Updated) < 0)
                        {
                            // We want to trigger the event multiple times if between two checks more comments were received
                            if (newerCommentAddedTimestamp.CompareTo(comment.Updated) < 0)
                            {
                                newerCommentAddedTimestamp = comment.Updated;
                            }

                            Trigger();
                        }
                    }

                    if (lastCommentAddedTimestamp.CompareTo(newerCommentAddedTimestamp) < 0)
                    {
                        lastCommentAddedTimestamp = newerCommentAddedTimestamp;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                    ErrorLog.AddError(ErrorType.Failure, Strings.Youtube_ErrorMonitoringComment);

                    return;
                }
            }

            timer.Start();
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new YoutubeCommentAddedConfig(videoID, Strings.YoutubeCommentAdded_Title); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as YoutubeCommentAddedConfig;

            if (config == null)
            {
                return;
            }
            videoID = config.VideoID;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.YoutubeCommentAdded_ConfigString, videoID);
        }

        #endregion
    }
}
