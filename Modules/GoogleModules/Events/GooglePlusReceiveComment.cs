using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using System.Timers;
using GoogleModules.Resources;
using GoogleModules.Wpf;
using GooglePlusLib.NET;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace GoogleModules.Events
{
    /// <summary>
    /// This class represents an event that will be triggered when a predefined user receives a comment on the selected activity.
    /// </summary>
    [DataContract]
    [MayhemModule("Google+: Receive Comment", "Triggers when a predefined user receives a comment on the selected activity")]
    public class GooglePlusReceiveComment : GooglePlusEventBase, IWpfConfigurable
    {
        [DataMember]
        private string activityLink;

        private string activityId;

        protected override void OnEnabling(EnablingEventArgs e)
        {
            base.OnEnabling(e);

            if (!e.Cancel)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        bool found = false;
                        string saveTokenActivities = string.Empty;

                        GPlusActivities activities = apiHelper.ListActivities();

                        do
                        {
                            saveTokenActivities = activities.nextPageToken;
                            foreach (GPlusActivity activity in activities.items)
                            {
                                if (activity.url.Equals(activityLink))
                                {
                                    activityId = activity.id;

                                    found = true;
                                    break;
                                }
                            }

                            if (found)
                            {
                                break;
                            }

                            activities = apiHelper.ListActivities(activities.nextPageToken);
                        } while (!string.IsNullOrEmpty(saveTokenActivities));

                        if (found)
                        {
                            StartTimer(100);
                        }
                        else
                        {
                            ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.General_Incorrect, Strings.General_ActivityID));

                            e.Cancel = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.AddError(ErrorType.Failure, Strings.GooglePlus_EventCouldntBeEnabled);
                        Logger.Write(ex);

                        e.Cancel = true;
                        return;
                    }
                });
            }
        }

        private DateTime GetTimestampMostRecentComment()
        {
            DateTime newLastCommentTime = DateTime.MinValue;
            string saveTokenComment = string.Empty;

            GPlusComments comments = apiHelper.ListComments(activityId);

            do
            {
                saveTokenComment = comments.nextPageToken;

                foreach (GPlusComment comment in comments.items)
                {
                    if (comment.published.CompareTo(newLastCommentTime) > 0)
                    {
                        newLastCommentTime = comment.published;
                    }
                }

                comments = apiHelper.ListComments(activityId, comments.nextPageToken);
            } while (!string.IsNullOrEmpty(saveTokenComment));

            return newLastCommentTime;
        }

        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            try
            {
                if (isFirstTime)
                {
                    lastAddedItemTimestamp = GetTimestampMostRecentComment();

                    isFirstTime = false;
                    timer.Interval = int.Parse(Strings.General_TimeInterval);

                    timer.Start();
                }
                else
                {
                    DateTime newLastCommentTime = GetTimestampMostRecentComment();

                    if (lastAddedItemTimestamp.CompareTo(newLastCommentTime) < 0)
                    {
                        lastAddedItemTimestamp = newLastCommentTime;

                        Trigger();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.GooglePlus_ErrorMonitoringNewComment);
                Logger.Write(ex);

                return;
            }

            timer.Start();
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new GooglePlusActivityLinkConfig(activityLink, Strings.GooglePlusReceiveComment_Title); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as GooglePlusActivityLinkConfig;

            profileId = config.ProfileID;
            activityLink = config.ActivityLink;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.ActivityLink_ConfigString, activityLink);
        }

        #endregion
    }
}
