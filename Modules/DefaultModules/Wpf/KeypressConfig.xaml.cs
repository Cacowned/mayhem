using System.Collections.Generic;
using System.Windows;
using DefaultModules.KeypressHelpers;
using MayhemCore.ModuleTypes;
using System.Windows.Controls;
using System.Diagnostics;
using MayhemWpf.UserControls;
using DefaultModules.Events;
using System.Windows.Forms;
using System.Windows.Input;

namespace DefaultModules.Wpf
{
    public partial class KeypressConfig : IWpfConfiguration
    {
        public HashSet<Keys> KeysToSave;
        public HashSet<System.Windows.Forms.Keys> keyCombination;

        private HashSet<Keys> keys_down = new HashSet<Keys>();
        private InterceptKeys interceptKeys;

        private bool shouldCheckValidity = false;

        public KeypressConfig(HashSet<System.Windows.Forms.Keys> MonitorKeysDown)
        {
            this.KeysToSave = MonitorKeysDown;
            keyCombination = new HashSet<Keys>(MonitorKeysDown);

            InitializeComponent();
        }

        public override string Title
        {
            get { return "Key Press"; }
        }

        public override void OnLoad()
        {
            Keypress.IsConfigOpen = true;

            interceptKeys = InterceptKeys.Instance;
            interceptKeys.AddRef();
            interceptKeys.OnKeyDown += InterceptKeys_OnInterceptKeyDown;
            interceptKeys.OnKeyUp += InterceptKeys_OnInterceptKeyUp;

            UpdateKeysDown(keyCombination);

            shouldCheckValidity = true;
        }

        private void InterceptKeys_OnInterceptKeyDown(Keys key)
        {
            if (!keys_down.Contains(key))
            {
                if (keys_down.Count == 0)
                {
                    keyCombination.Clear();
                }
                keys_down.Add(key);
                keyCombination.Add(key);

                UpdateKeysDown(keyCombination);
            }
        }

        private void InterceptKeys_OnInterceptKeyUp(Keys key)
        {
            if (keys_down.Contains(key))
            {
                keys_down.Remove(key);
            }
        }

        private void UpdateKeysDown(HashSet<System.Windows.Forms.Keys> keys)
        {
            string str = "";
            foreach (System.Windows.Forms.Keys key in keys)
            {
                if (str == "")
                {
                    str = key.ToString();
                }
                else
                {
                    str += " + " + key.ToString();
                }
            }
            textBoxKeys.Text = str;

            CanSave = keys.Count > 0;
            if (shouldCheckValidity)
            {
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public override void OnClosing()
        {
            interceptKeys.RemoveRef();
            Keypress.IsConfigOpen = false;
            interceptKeys.OnKeyDown -= InterceptKeys_OnInterceptKeyDown;
            interceptKeys.OnKeyUp -= InterceptKeys_OnInterceptKeyUp;
        }

        public override bool OnSave()
        {
            if (keyCombination.Count == 0)
            {
                System.Windows.MessageBox.Show("You must have at least one key pressed");
                return false;
            }
            KeysToSave.Clear();
            foreach (Keys key in keyCombination)
            {
                KeysToSave.Add(key);
            }
            return true;
        }

        private void IWpfConfiguration_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // do stuff here.          

                e.Handled = true;
            }
        }
    }
}
