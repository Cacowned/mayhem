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
using System.Collections.Generic;
using FacebookModules.Wpf.UserControls;

namespace FacebookModules.Reactions
{
    [DataContract]
    [MayhemModule("Facebook: Status Update", "Updates your Facebook status")]
    public class UpdateStatus : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string token;

        [DataMember]
        private string userName;

        [DataMember]
        private string myStatus;

        private FacebookClient fb;
        

        public override void Perform()
        {
            if (Utilities.ConnectedToInternet())
            {
                var statusArgs = new SortedDictionary<string, string>();
                statusArgs["message"] = myStatus;
                try
                {
                    fb.Post("/me/feed", statusArgs);
                }
                catch
                {
                    // duplicate status posted
                }
            }
            else
            {
                ErrorLog.AddError(ErrorType.Warning, String.Format(Strings.Internet_NotConnected, "Facebook Status"));
            }
        }

        protected override void OnLoadDefaults()
        {
            myStatus = "";
        }

        protected override void OnLoadFromSaved()
        {
            fb = new FacebookClient(token);
        }

        /// <summary>
        /// Checks if there is Internet, if so checks the current User's Facebook feed
        /// and triggers if there is a new post to their wall
        /// </summary>
        private void CheckFacebook(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// The configuration string
        /// </summary>
        /// <returns>Watching {User Name}'s wall for new posts</returns>
        public string GetConfigString()
        {
            return String.Format("Updates Facebook Status");
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            FacebookConfigDebug debug = configurationControl as FacebookConfigDebug;
            StatusConfig config = debug.ControlItem as StatusConfig;

            token = debug.TokenProp;
            fb = new FacebookClient(token);

            myStatus = config.StatusProp;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new FacebookConfigDebug(token, "Status Update", new StatusConfig(myStatus)); }
        }
    }
}

