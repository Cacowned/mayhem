using System;

namespace MayhemCore
{
    /// <summary>
    /// EventArguments to OnDisabled for Modules
    /// </summary>
    public class DisabledEventArgs : EventArgs
    {
        /// <summary>
        /// True if the module is being disabled because a configuration window
        /// was opened.
        /// </summary>
        public bool IsConfiguring
        {
            get;
            private set;
        }

        public DisabledEventArgs(bool isConfiguring)
        {
            this.IsConfiguring = isConfiguring;
        }
    }
}
