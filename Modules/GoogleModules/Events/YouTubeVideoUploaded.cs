using System;
using System.Runtime.Serialization;
using System.Timers;
using Google.YouTube;
using GoogleModules.Resources;
using GoogleModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace GoogleModules.Events
{
    /// <summary>
    /// This class represents an event that will be triggered when a new video is uploaded by a predefined user.
    /// </summary>
    [DataContract]
    [MayhemModule("YouTube: Video Uploaded", "Triggers when a predefined user uploads a video")]
    public class YouTubeVideoUploaded : YouTubeVideoEventBase, IWpfConfigurable
    {
        protected override void OnEnabling(EnablingEventArgs e)
        {
            base.OnEnabling(e);

            if (!e.Cancel)
            {
                try
                {
                    foreach (Video entry in currentVideoList)
                    {
                        if (lastVideoAddedTimestamp.CompareTo(entry.YouTubeEntry.Published) < 0)
                        {
                            lastVideoAddedTimestamp = entry.YouTubeEntry.Published;
                        }
                    }

                    StartTimer(int.Parse(Strings.General_TimeInterval));
                }
                catch (Exception ex)
                {
                    ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.GooglePlus_ErrorMonitoringYoutubeUploadedVideos, username));
                    Logger.Write(ex);

                    e.Cancel = true;
                    return;
                }
            }
        }

        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            try
            {
                videoFeed = request.Get<Video>(feedUri);

                DateTime newerAddedVideoTimestamp = lastVideoAddedTimestamp;

                foreach (Video entry in videoFeed.Entries)
                {
                    if (lastVideoAddedTimestamp.CompareTo(entry.YouTubeEntry.Published) < 0)
                    {
                        // We want to trigger the event multiple times if between two checks more that one video was uploaded.
                        if (newerAddedVideoTimestamp.CompareTo(entry.YouTubeEntry.Published) < 0)
                        {
                            newerAddedVideoTimestamp = entry.YouTubeEntry.Published;
                        }

                        Trigger();
                    }
                }

                if (lastVideoAddedTimestamp.CompareTo(newerAddedVideoTimestamp) < 0)
                {
                    lastVideoAddedTimestamp = newerAddedVideoTimestamp;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.GooglePlus_ErrorMonitoringYoutubeUploadedVideos, username));
                Logger.Write(ex);

                return;
            }

            timer.Start();
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new YouTubeUsernameConfig(username, Strings.YouTubeVideoUploaded_Title); }
        }
    }
}
