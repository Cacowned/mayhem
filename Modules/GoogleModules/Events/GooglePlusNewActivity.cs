﻿using System;
using System.Globalization;
using System.Runtime.Serialization;
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
    /// This class represents an event that will be triggered when a new activity is added to the public feed of a predefined user.
    /// </summary>
    [DataContract]
    [MayhemModule("Google+: New Activity", "Triggers when a predefined user posts in the Public Feed an activity")]
    public class GooglePlusNewActivity : GooglePlusEventBaseClass, IWpfConfigurable
    {
        protected override void OnEnabling(EnablingEventArgs e)
        {
            base.OnEnabling(e);

            if (!e.Cancel)
            {
                StartTimer(100);
            }
        }

        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            try
            {
                DateTime newLastActivityTime = new DateTime();

                GPlusActivities activities = apiHelper.ListActivities();

                if (isFirstTime)
                {
                    if (activities.items.Length == 0)
                    {
                        // The user hasn't shared anything 
                        lastAddedItemTimestamp = DateTime.MinValue;
                    }
                    else
                    {
                        GPlusActivity activity = activities.items[0];
                        lastAddedItemTimestamp = activity.published;
                    }

                    isFirstTime = false;
                    timer.Interval = int.Parse(Strings.General_TimeInterval);

                    timer.Start();

                    return;
                }

                string saveTokenActivity = string.Empty;
                bool finish = false;

                do
                {
                    saveTokenActivity = activities.nextPageToken;

                    foreach (GPlusActivity activity in activities.items)
                    {
                        if (activity.published.CompareTo(lastAddedItemTimestamp) > 0)
                        {
                            Trigger();

                            if (activity.published.CompareTo(newLastActivityTime) > 0)
                            {
                                newLastActivityTime = activity.published;
                            }
                        }
                        else
                        {
                            // We don't have any new event.
                            finish = true;
                            break;
                        }
                    }

                    if (finish)
                    {
                        break;
                    }

                    activities = apiHelper.ListActivities(saveTokenActivity);
                } while (!string.IsNullOrEmpty(saveTokenActivity));

                if (lastAddedItemTimestamp.CompareTo(newLastActivityTime) < 0)
                {
                    lastAddedItemTimestamp = newLastActivityTime;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.GooglePlus_ErrorMonitoringNewActivity);
                Logger.Write(ex);

                return;
            }

            timer.Start();
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new GooglePlusProfileIDConfig(profileId, Strings.GooglePlusNewActivity_Title); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as GooglePlusProfileIDConfig;

            if (config == null)
            {
                return;
            }

            profileId = config.ProfileID;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.ProfileID_ConfigString, profileId);
        }

        #endregion
    }
}
