using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using DefaultModules.KeypressHelpers;
using System.Diagnostics;
using DefaultModules.Wpf;
using System.Windows;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace DefaultModules.Actions
{
    // Because of it's support for shifts and stuff, it won't work in CLI mode.
    [Serializable]
    public class Keypress : ActionBase, IWpf, ISerializable
    {
        public const string TAG = "[Key Press]";

        private HashSet<System.Windows.Forms.Keys> monitor_keys_down = null;

        private HashSet<System.Windows.Forms.Keys> keys_down = new HashSet<System.Windows.Forms.Keys>();
        private InterceptKeys.KeyDownHandler keyDownHandler = null;
        private InterceptKeys.KeyUpHandler keyUpHandler = null;

        private InterceptKeys interceptKeys;

        public Keypress()
            : base("Key Press", "This trigger fires on a predefined key press") {
                
            Setup();
            // Default key to add
            monitor_keys_down.Add(Keys.Enter);
            SetConfigString();
        }

        protected void Setup()
        {
            hasConfig = true;

            interceptKeys = InterceptKeys.GetInstance();

            monitor_keys_down = new HashSet<System.Windows.Forms.Keys>();

            keyDownHandler = new InterceptKeys.KeyDownHandler(Intercept_key_down);
            keyUpHandler = new InterceptKeys.KeyUpHandler(Intercept_key_up);
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

        private void Intercept_key_down(object sender, System.Windows.Forms.KeyEventArgs e) {
            keys_down.Add(e.KeyCode);

            if (Keysets_Equal() && Enabled) {
                OnActionActivated();
            }

        }

        private bool Keysets_Equal() {
            if (keys_down.Count == monitor_keys_down.Count) {
                foreach (System.Windows.Forms.Keys k in monitor_keys_down) {
                    bool foundEqiv = false;
                    foreach (System.Windows.Forms.Keys l in keys_down) {
                        if (l == k) { foundEqiv = true; break; }
                    }

                    if (foundEqiv == false)
                        return false;
                }
                return true;
            } else {
                return false;
            }
        }

        private void Intercept_key_up(object sender, System.Windows.Forms.KeyEventArgs e) {
            keys_down.Remove(e.KeyCode);
        }


        #region Serialization
        /** <summary>
          *  De-Serialization constructor
          * </summary> 
          * */
        public Keypress(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Setup();

            string[] key_strings = null;
            try
            {
                key_strings = info.GetValue("monitor_keys", typeof(object)) as string[];
            }
            catch (SerializationException ex)
            {
                Debug.WriteLine(TAG + "Serialization Exception !!!!!\n" + ex);
            }

            if (key_strings == null)
            {
                Debug.WriteLine(TAG + "monitor keys is either null or deserialization is broken!!");
            }
            else
            {
                for (int i = 0; i < key_strings.Length; i++)
                {
                    Keys k = (Keys)Enum.Parse(typeof(Keys), key_strings[i]);
                    monitor_keys_down.Add(k);
                }
            }

            SetConfigString();
        }

        /** <summary>
         *  Packaged values for serialzation are stored here
         *  Warning:
         *   -- override with "new" keyword
         *   -- be sure to call the base getObjectData
         * </summary>
         * */
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //throw new NotImplementedException();

            base.GetObjectData(info, context);

            string[] keyStrings = new string[monitor_keys_down.Count];
            System.Windows.Forms.Keys[] keys = monitor_keys_down.ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                keyStrings[i] = keys[i].ToString();
            }

            info.AddValue("monitor_keys", keyStrings);
        }
        #endregion
    }
}
