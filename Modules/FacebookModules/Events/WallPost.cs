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
        /// <summary>
        /// Checks for new wall posts
        /// </summary>
        public override string WhatToCheck
        {
            get
            {
                return "/me/feed";
            }
        }

        /// <summary>
        /// The configuration string
        /// </summary>
        /// <returns>Watching {User Name}'s wall for new posts</returns>
        public string GetConfigString()
        {
            return String.Format("Watching {0}'s wall", userName);
        }
    }
}

