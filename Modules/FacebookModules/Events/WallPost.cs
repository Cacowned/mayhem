using System;
using System.Runtime.Serialization;
using System.Windows.Threading;
using Facebook;
using FacebookModules.LowLevel;
using FacebookModules.Resources;
using FacebookModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace FacebookModules.Events
{
    [DataContract]
    [MayhemModule("Facebook", "Watches for new wall posts")]
    public class WallPost : FacebookEventBase, IWpfConfigurable
    {
        [DataMember]
        private string token;

        private DispatcherTimer timer;
        private Facebook.FacebookOAuthResult api;

        private string userName;
        private string postId;

        protected override void OnLoadDefaults()
        {
        }

        protected override void OnAfterLoad()
        {
            //api = new Facebook.FacebookAPI(token);
            postId = null;

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
                var fb = new FacebookClient(token); //api.AccessToken);
                dynamic result = fb.Get("/me/feed");

                // get the id of the most recent wall post
                string latestPostId = result.data[0].id;

                if (postId == null)
                    postId = latestPostId;
                else if (!latestPostId.Equals(postId) || true)
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

        /// <summary>
        /// The configuration string
        /// </summary>
        /// <returns>Watching {User Name}'s wall for new posts</returns>
        public string GetConfigString()
        {
            return String.Format("Watching {0}'s wall for new posts", userName);
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = (WallPostConfig)configurationControl;
            token = config.TokenProp;
            userName = config.UserNameProp;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new WallPostConfig(); }
        }
    }
}

