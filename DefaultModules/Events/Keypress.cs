using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using DefaultModules.KeypressHelpers;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemDefaultStyles.UserControls;
using System.Windows.Controls;

namespace DefaultModules.Events
{
    // Because of it's support for shifts and stuff, it won't work in CLI mode.
    [DataContract]
    public class Keypress : EventBase, IWpf
    {
        public const string TAG = "[Key Press]";

        private InterceptKeys interceptKeys;

        public static bool IsConfigOpen = false;

        [DataMember]
        private HashSet<System.Windows.Forms.Keys> MonitorKeysDown { get; set; }

        public Keypress()
            : base("Key Press", "This event fires on a predefined key press") { }

        protected override void Initialize()
        {
            base.Initialize();

            hasConfig = true;

            // Set our defaults
            MonitorKeysDown = new HashSet<System.Windows.Forms.Keys>();

            interceptKeys = InterceptKeys.Instance;

            SetConfigString();
        }

        public IWpfConfig ConfigurationControl
        {
            get { return new KeypressConfig(MonitorKeysDown); }
        }

        public void OnSaved(IWpfConfig configurationControl)
        {
            interceptKeys.RemoveCombinationHandler(MonitorKeysDown, OnKeyCombinationActivated);
            MonitorKeysDown = (configurationControl as KeypressConfig).KeysToSave;
            interceptKeys.AddCombinationHandler(MonitorKeysDown, OnKeyCombinationActivated);
            SetConfigString();
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

            interceptKeys.AddCombinationHandler(MonitorKeysDown, OnKeyCombinationActivated);
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
