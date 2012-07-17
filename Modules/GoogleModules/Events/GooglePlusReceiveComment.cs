using System;
using System.Timers;
using GoogleModules.Resources;
using GoogleModules.Wpf;
using GooglePlusLib.NET;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace GoogleModules.Events
{
    [MayhemModule("Google+: Receive Comment", "Triggers when a predefined user receives a comment")]
    public class GooglePlusReceiveComment : GooglePlusBaseClass, IWpfConfigurable
    {
        private DateTime GetTimestampMostRecentComment()
        {
            GPlusActivities activities = apiHelper.ListActivities();

            DateTime newLastCommentTime = new DateTime();

            string saveTokenActivity = string.Empty;

            do
            {
                saveTokenActivity = activities.nextPageToken;

                foreach (GPlusActivity activity in activities.items)
                {
                    GPlusComments comments = apiHelper.ListComments(activity.id);

                    string saveTokenComment = string.Empty;

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

                        comments = apiHelper.ListComments(activity.id, comments.nextPageToken);
                    } while (!string.IsNullOrEmpty(saveTokenComment));
                }

                activities = apiHelper.ListActivities(activities.nextPageToken);
            } while (!string.IsNullOrEmpty(activities.nextPageToken));

            return newLastCommentTime;
        }

        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            try
            {
                GPlusActivities activities = apiHelper.ListActivities();

                if (isFirstTime)
                {
                    lastAddedItemTimestamp = GetTimestampMostRecentComment();

                    isFirstTime = false;
                    timer.Interval = 5000;

                    timer.Start();

                    return;
                }

                DateTime newLastCommentTime = GetTimestampMostRecentComment();

                if (lastAddedItemTimestamp.CompareTo(newLastCommentTime) < 0)
                {
                    lastAddedItemTimestamp = newLastCommentTime;

                    Trigger();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.GooglePlus_ErrorMonitoringNewComment);
                Logger.Write(ex);
            }

            timer.Start();
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new GooglePlusProfileIDConfig(profileId, Strings.GooglePlusNewCommentTitle); }
        }
    }
}
