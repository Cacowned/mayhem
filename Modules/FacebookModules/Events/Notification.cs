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
    [MayhemModule("Facebook", "Watches for new notifications")]
    public class Notification : FacebookEventBase, IWpfConfigurable
    {
        /// <summary>
        /// Checks for new notifications
        /// </summary>
        public override string WhatToCheck
        {
            get
            {
                return "/me/notifications";
            }
        }

        /// <summary>
        /// Sets the title as "Facebook - Notification"
        /// </summary>
        public override string Title
        {
            get { return "Notification"; }
        }

        /// <summary>
        /// The configuration string
        /// </summary>
        /// <returns>Watching {User Name}'s wall for new posts</returns>
        public string GetConfigString()
        {
            return String.Format("Watching {0}'s notifications", userName);
        }
    }
}

