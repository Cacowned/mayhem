using System;
using System.Runtime.Serialization;
using System.Windows.Threading;
using Facebook;
using FacebookModules.Wpf;
using MayhemCore;
using MayhemWpf.UserControls;
using FacebookModules.LowLevel;
using FacebookModules.Resources;
using System.Windows;

namespace FacebookModules
{
    /// <summary>
    /// Base class for all Facebook Events
    /// </summary>
    [DataContract]
    public abstract class FacebookEventBase : EventBase
    {
        [DataMember]
        public string token;

        [DataMember]
        public string userName;

        public FacebookClient fb;
        public DispatcherTimer timer;
        public string postId;

        public abstract string WhatToCheck { get; }

        public WpfConfiguration ConfigurationControl
        {
            get { return new FacebookConfigDebug(token, null); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = (FacebookConfigDebug)configurationControl;
            token = config.TokenProp;

            fb = new FacebookClient(token);

            if (userName == null)
            {
                dynamic res = fb.Get("/me");
                userName = res.name;
            }
        }

        protected override void OnAfterLoad()
        {
            postId = null;

            if (token != null)
            {
                fb = new FacebookClient(token);
            }

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Tick += CheckFacebook;
        }

        /// <summary>
        /// Checks if there is Internet, if so checks the current User's Facebook feed
        /// and triggers if there is a new post to their wall
        /// </summary>
        private void CheckFacebook(object sender, EventArgs e)
        {
            if (Utilities.ConnectedToInternet())
            {
                dynamic result = fb.Get(WhatToCheck);

                // get the id of the most recent wall post
                string latestPostId = result.data[0].id;

                if (postId == null)
                {
                    postId = latestPostId;
                }
                else if (!latestPostId.Equals(postId)) // remove later TRUE for testing
                {
                    postId = latestPostId;
                    Trigger();
                }
            }
            else
            {
                ErrorLog.AddError(ErrorType.Warning, String.Format(Strings.Internet_NotConnected, "Facebook"));
            }
        }

        #region Timer
        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (token != null)
                timer.Start();
            else
                ErrorLog.AddError(ErrorType.Warning, String.Format(Strings.Internet_NotConnected, "Facebook"));
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            timer.Stop();
        }
        #endregion
    }
}
