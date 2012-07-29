using System;
using System.Net;
using System.Runtime.Serialization;
using System.Timers;
using GoogleModules.Resources;
using GooglePlusLib.NET;
using MayhemCore;

namespace GoogleModules.Events
{
    /// <summary>
    /// This is the base class for the Google+ Events.
    /// </summary>
    [DataContract]
    public abstract class GooglePlusEventBaseClass : EventBase
    {
        [DataMember]
        protected string profileId;

        protected DateTime lastAddedItemTimestamp;

        protected Timer timer;
        protected bool isFirstTime;

        protected GooglePlusAPIHelper apiHelper;

        protected override void OnEnabling(EnablingEventArgs e)
        {
            isFirstTime = true;

            try
            {
                apiHelper = new GooglePlusAPIHelper(profileId, Strings.GooglePlus_ApiKey);

                // Trying to get the current user information in order to see if the entered profileId exists.
                apiHelper.GetPerson();
            }
            catch (WebException ex)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.General_Incorrect, Strings.General_ProfileID));
                Logger.Write(ex);

                e.Cancel = true;
                return;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.GooglePlus_EventCouldntBeEnabled);
                Logger.Write(ex);

                e.Cancel = true;
                return;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Elapsed -= timer_Elapsed;
                timer.Dispose();
            }
        }

        protected void StartTimer(int interval)
        {
            timer = new Timer();
            timer.Interval = interval;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            timer.Start();
        }

        protected abstract void timer_Elapsed(object sender, ElapsedEventArgs e);
    }
}
