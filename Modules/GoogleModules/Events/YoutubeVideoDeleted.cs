using System;
using System.Collections.Generic;
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
    /// This class represents an event that will be triggered when a user deletes a video.
    /// </summary>
    [DataContract]
    [MayhemModule("YouTube: Video Deleted", "Triggers when a predefined user deletes a video")]
    public class YouTubeVideoDeleted : YouTubeVideoEventBase, IWpfConfigurable
    {
        protected override void OnEnabling(EnablingEventArgs e)
        {
            base.OnEnabling(e);

            if (!e.Cancel)
            {
                StartTimer(int.Parse(Strings.General_TimeInterval));
            }
        }

        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            try
            {
                List<Video> auxVideoList = new List<Video>();
                bool found = false;

                videoFeed = request.Get<Video>(feedUri);

                foreach (Video video in currentVideoList)
                {
                    foreach (Video entry in videoFeed.Entries)
                    {
                        found = false;

                        if (entry.VideoId.Equals(video.VideoId))
                        {
                            found = true;

                            break;
                        }
                    }

                    if (!found)
                    {
                        Trigger();

                        auxVideoList.Add(video);
                    }
                }

                foreach (Video video in auxVideoList)
                {
                    currentVideoList.Remove(video);
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
            get { return new YouTubeUsernameConfig(username, Strings.YouTubeVideoDeleted_Title); }
        }
    }
}
