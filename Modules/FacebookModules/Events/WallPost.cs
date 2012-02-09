using System.Runtime.Serialization;
using Facebook;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using FacebookModules.Wpf;
using System.Windows.Threading;
using System;
using FacebookModules.LowLevel;
using FacebookModules.Resources;

namespace FacebookModules.Events
{
    [DataContract]
    [MayhemModule("Facebook", "Walls post Facebook app.")]
    public class WallPost : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string token;

        private DispatcherTimer timer;

        private Facebook.FacebookAPI api;
        private string userName;
        private string postId;

        protected override void OnLoadDefaults()
        {
        }

        protected override void OnAfterLoad()
        {
            api = new Facebook.FacebookAPI(token);
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
                JSONObject me = api.Get("/me/feed");
                string latestPostId = me.Dictionary["data"].Array[0].Dictionary["id"].String;

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

