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
    [MayhemModule("Facebook", "Watches for new messages")]
    public class Message : FacebookEventBase, IWpfConfigurable
    {
        /// <summary>
        /// Checks for new messages
        /// </summary>
        public override string WhatToCheck
        {
            get
            {
                return "/me/inbox";
            }
        }

        /// <summary>
        /// The configuration string
        /// </summary>
        /// <returns>Watching {User Name}'s wall for new posts</returns>
        public string GetConfigString()
        {
            return String.Format("Watching {0}'s inbox", userName);
        }
    }
}

