using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using DefaultModules.KeypressHelpers;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DefaultModules.Events
{
    // Because of it's support for shifts and stuff, it won't work in CLI mode.
    [DataContract]
    [MayhemModule("Key Press", "This event fires on a predefined key press")]
    public class KeyPress : EventBase, IWpfConfigurable
    {
        private InterceptKeys interceptKeys;

        // If there are multiple keypress events, we want to disable their activation 
        // when one of them is being configured
        public static bool IsConfigOpen { get; set; }

        private Thread mainThread;

        #region Configuration

        [DataMember]
        private HashSet<System.Windows.Forms.Keys> MonitorKeysDown;

        #endregion

        public KeyPress()
        {
            MonitorKeysDown = new HashSet<System.Windows.Forms.Keys>();
        }

        protected override void Initialize()
        {
            interceptKeys = InterceptKeys.Instance;
            mainThread = Thread.CurrentThread;
        }

        public string GetConfigString()
        {
            StringBuilder b = new StringBuilder();

            foreach (System.Windows.Forms.Keys k in MonitorKeysDown)
            {
                if (b.Length == 0)
                {
                    b.Append(k.ToString());
                }
                else
                {
                    b.Append(", " + k.ToString());
                }
            }

            return "Keys: " + b.ToString();
        }

        #region Configuration Views
        public WpfConfiguration ConfigurationControl
        {
            get { return new KeypressConfig(MonitorKeysDown); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            interceptKeys.RemoveCombinationHandler(MonitorKeysDown, OnKeyCombinationActivated);
            MonitorKeysDown = (configurationControl as KeypressConfig).KeysToSave;
            interceptKeys.AddCombinationHandler(MonitorKeysDown, OnKeyCombinationActivated);
        }

        #endregion

        private void OnKeyCombinationActivated()
        {
            if (!IsConfigOpen)
            {
                if (IsEnabled)
                {
                    Trigger();
                }
            }
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            Dispatcher.FromThread(mainThread).Invoke((Action)delegate
            {
                interceptKeys.AddCombinationHandler(MonitorKeysDown, OnKeyCombinationActivated);
            });
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            interceptKeys.RemoveCombinationHandler(MonitorKeysDown, OnKeyCombinationActivated);
        }
    }
}
