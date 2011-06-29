using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using DefaultModules.KeypressHelpers;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;

namespace DefaultModules.Actions
{
    // Because of it's support for shifts and stuff, it won't work in CLI mode.
    [DataContract]
    public class Keypress : ActionBase, IWpf
    {
        public const string TAG = "[Key Press]";

        private HashSet<System.Windows.Forms.Keys> keys_down = new HashSet<System.Windows.Forms.Keys>();
        private InterceptKeys.KeyDownHandler keyDownHandler = null;
        private InterceptKeys.KeyUpHandler keyUpHandler = null;

        private InterceptKeys interceptKeys;

        [DataMember]
        private HashSet<System.Windows.Forms.Keys> monitor_keys_down = null;

        public Keypress()
            : base("Key Press", "This trigger fires on a predefined key press")
        {

            hasConfig = true;

            // Set our defaults
            monitor_keys_down = new HashSet<System.Windows.Forms.Keys>();
            monitor_keys_down.Add(Keys.Enter);

            interceptKeys = InterceptKeys.GetInstance();

            keyDownHandler = new InterceptKeys.KeyDownHandler(Intercept_key_down);
            keyUpHandler = new InterceptKeys.KeyUpHandler(Intercept_key_up);

            
            SetConfigString();
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            SetConfigString();
        }

        public void WpfConfig()
        {
            var window = new KeypressConfig();
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            foreach (System.Windows.Forms.Keys k in monitor_keys_down)
            {
                window.Deserialize_AddKey(k);
            }

            window.ShowDialog();

            if (window.DialogResult == true)
            {
                monitor_keys_down = window.keys_down;

                SetConfigString();
            }

        }

        protected void SetConfigString()
        {
            StringBuilder b = new StringBuilder();

            foreach (Keys k in monitor_keys_down)
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

            InterceptKeys.OnInterceptKeyDown += keyDownHandler;
            InterceptKeys.OnInterceptKeyUp += keyUpHandler;
        }

        public override void Disable()
        {
            base.Disable();

            InterceptKeys.OnInterceptKeyDown -= keyDownHandler;
            InterceptKeys.OnInterceptKeyUp -= keyUpHandler;
        }

        private void Intercept_key_down(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            keys_down.Add(e.KeyCode);

            if (Keysets_Equal() && Enabled)
            {
                OnActionActivated();
            }

        }

        private bool Keysets_Equal()
        {
            if (keys_down.Count == monitor_keys_down.Count)
            {
                foreach (System.Windows.Forms.Keys k in monitor_keys_down)
                {
                    bool foundEqiv = false;
                    foreach (System.Windows.Forms.Keys l in keys_down)
                    {
                        if (l == k) { foundEqiv = true; break; }
                    }

                    if (foundEqiv == false)
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Intercept_key_up(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            keys_down.Remove(e.KeyCode);
        }
    }
}
