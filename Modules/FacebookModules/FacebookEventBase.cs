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

        public string postId;
        public int commentLength;

        public FacebookClient fb;
        public DispatcherTimer timer;

        public bool showError;

        public abstract string WhatToCheck { get; }
        public abstract string Title { get; }

        public WpfConfiguration ConfigurationControl
        {
            get { return new FacebookConfigDebug(token, Title, null); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = (FacebookConfigDebug)configurationControl;
            token = config.TokenProp;

            fb = new FacebookClient(token);

            dynamic res = fb.Get("/me");
            userName = res.name;
        }

        protected override void OnAfterLoad()
        {

            if (token != null)
            {
                fb = new FacebookClient(token);
            }

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Tick += CheckFacebook;

            commentLength = -1;
            showError = true; // change var name
        }

        /// <summary>
        /// Checks if there is Internet, if so checks the current User's Facebook feed
        /// and triggers if there is a new post to their wall
        /// </summary>
        private void CheckFacebook(object sender, EventArgs e)
        {
            if (Utilities.ConnectedToInternet())
            {
                showError = true;
                dynamic result = fb.Get(WhatToCheck);

                // get the id of the most recent post
                string latestPostId = string.Empty;
                int latestComments = 0;
                if (!(WhatToCheck.Equals("/me/notifications") && result.Values.Count == 2))
                {
                    try
                    {
                        try
                        {
                            latestComments = result.data[0].comments.data[0].Count;
                        }
                        catch
                        {
                            // only one message in the feed
                        }
                        latestPostId = result.data[0].id;
                    }
                    catch
                    {
                        // emtpy inbox
                    }
                }
                else
                {
                    postId = "[]";
                }

                if (!latestPostId.Equals(string.Empty))
                {
                    // get the id of the first post
                    if (postId == null || (commentLength == -1 && WhatToCheck.Equals("/me/inbox")))
                    {
                        postId = latestPostId;
                        try
                        {
                            commentLength = result.data[0].comments.data[0].Count;
                        }
                        catch
                        {

                        }
                    }
                    else if (!latestPostId.Equals(postId) || (WhatToCheck.Equals("/me/inbox") && commentLength != latestComments))
                    {
                        postId = latestPostId;
                        commentLength = latestComments;
                        Trigger();
                    }
                }
            }
            else if (showError)
            {
                ErrorLog.AddError(ErrorType.Warning, String.Format(Strings.Internet_NotConnected, "Facebook"));
                showError = false;
            }
        }

        #region Timer
        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (token != null)
                timer.Start();
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            timer.Stop();
        }
        #endregion
    }
}
