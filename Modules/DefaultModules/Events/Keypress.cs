using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using DefaultModules.KeypressHelpers;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemDefaultStyles.UserControls;

namespace DefaultModules.Events
{
    // Because of it's support for shifts and stuff, it won't work in CLI mode.
    [DataContract]
    [MayhemModule("Key Press", "This event fires on a predefined key press")]
    public class Keypress : EventBase, IWpfConfigurable
    {
        private InterceptKeys interceptKeys;

        // If there are multiple keypress events, we want to disable their activation 
        // when one of them is being configured
        public static bool IsConfigOpen = false;

        Thread mainThread;

        [DataMember]
        private HashSet<System.Windows.Forms.Keys> MonitorKeysDown
        {
            get;
            set;
        }

        protected override void Initialize()
        {
            base.Initialize();

            interceptKeys = InterceptKeys.Instance;
            mainThread = Thread.CurrentThread;

            MonitorKeysDown = new HashSet<System.Windows.Forms.Keys>();
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { return new KeypressConfig(MonitorKeysDown); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            interceptKeys.RemoveCombinationHandler(MonitorKeysDown, OnKeyCombinationActivated);
            MonitorKeysDown = (configurationControl as KeypressConfig).KeysToSave;
            interceptKeys.AddCombinationHandler(MonitorKeysDown, OnKeyCombinationActivated);
        }

        public override void SetConfigString()
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

            ConfigString = "Keys: " + b.ToString();
        }

        public override void Enable()
        {
            base.Enable();
            Dispatcher.FromThread(mainThread).Invoke((Action)delegate
            {
                interceptKeys.AddCombinationHandler(MonitorKeysDown, OnKeyCombinationActivated);
            });
        }

        public override void Disable()
        {
            base.Disable();

            interceptKeys.RemoveCombinationHandler(MonitorKeysDown, OnKeyCombinationActivated);
        }

        void OnKeyCombinationActivated()
        {
            if (!IsConfigOpen)
            {
                if (Enabled)
                {
                    OnEventActivated();
                }
            }
        }
    }
}
