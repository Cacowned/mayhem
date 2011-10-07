using System;

namespace MayhemCore
{
    /// <summary>
    /// EventArguments to OnEnabling for Modules
    /// </summary>
    public class EnablingEventArgs : EventArgs
    {
        /// <summary>
        /// Set to true in order to cancel the connection with this 
        /// module from enabling.
        /// </summary>
        public bool Cancel
        {
            get;
            set;
        }

        /// <summary>
        /// True if the module is re-enabling after being configured 
        /// </summary>
        public bool WasConfiguring
        {
            get;
            private set;
        }

        public EnablingEventArgs(bool wasConfiguring)
        {
            this.Cancel = false;
            this.WasConfiguring = wasConfiguring;
        }
    }
}
