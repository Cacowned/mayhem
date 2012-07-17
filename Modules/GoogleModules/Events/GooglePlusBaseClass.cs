using System;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;
using System.Timers;
using GoogleModules.Resources;
using GoogleModules.Wpf;
using GooglePlusLib.NET;
using MayhemCore;
using MayhemWpf.UserControls;

namespace GoogleModules.Events
{
    /// <summary>
    /// This is the base class for the Google+ Events.
    /// </summary>
    [DataContract]
    public abstract class GooglePlusBaseClass : EventBase
    {
        /// <summary>
        /// The profile id setted by the user.
        /// </summary>
        [DataMember]
        protected string profileId;

        protected DateTime lastAddedItemTimestamp;

        protected Timer timer;
        protected bool isFirstTime;

        protected GooglePlusAPIHelper apiHelper;

        /// <summary>
        /// This method is called when the event is enabling and initializes the needed object and checks if the selected profileId exists.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            try
            {
                apiHelper = new GooglePlusAPIHelper(profileId, Strings.GooglePlus_ApiKey);

                // Trying to get the current user information in order to see if the entered profileId exists and starts the timer
                GPlusPerson person = apiHelper.GetPerson();
            }
            catch (WebException ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.GooglePlus_ProfileIDIncorrect);
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

            timer = new Timer();
            timer.Interval = 100;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            isFirstTime = true;

            timer.Start();
        }

        /// <summary>
        /// This method is called after the event is disabled and stops the timer.
        /// </summary>
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Elapsed -= timer_Elapsed;
                timer.Dispose();
            }
        }

        /// <summary>
        /// This method will be implemented by the classes that inherit this class and will be called when the timer.Elapsed event will be raised.
        /// </summary>
        protected abstract void timer_Elapsed(object sender, ElapsedEventArgs e);

        #region IWpfConfigurable Methods

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
