using System.Collections.Generic;
using System.Windows;
using DefaultModules.KeypressHelpers;
using MayhemCore.ModuleTypes;
using System.Windows.Controls;
using System.Diagnostics;
using MayhemDefaultStyles.UserControls;
using DefaultModules.Events;
using System.Windows.Forms;
using System.Windows.Input;

namespace DefaultModules.Wpf
{
    public partial class KeypressConfig : IWpfConfiguration
    {
        public HashSet<System.Windows.Forms.Keys> KeysToSave;
        private HashSet<System.Windows.Forms.Keys> keys_down = new HashSet<System.Windows.Forms.Keys>();
        private InterceptKeys interceptKeys;

        public KeypressConfig(HashSet<System.Windows.Forms.Keys> MonitorKeysDown)
        {
            InitializeComponent();

            this.KeysToSave = MonitorKeysDown;
        }

        public override void OnLoad()
        {
            Keypress.IsConfigOpen = true;

            interceptKeys = InterceptKeys.Instance;
            interceptKeys.OnKeyDown += InterceptKeys_OnInterceptKeyDown;
            interceptKeys.OnKeyUp += InterceptKeys_OnInterceptKeyUp;

            UpdateKeysDown(KeysToSave);

            textInvalid.Visibility = Visibility.Collapsed;
        }

        void InterceptKeys_OnInterceptKeyDown(Keys key)
        {
            if (!keys_down.Contains(key))
            {
                if (keys_down.Count == 0)
                {
                    KeysToSave.Clear();
                }
                keys_down.Add(key);
                KeysToSave.Add(key);

                UpdateKeysDown(KeysToSave);
            }
        }

        void InterceptKeys_OnInterceptKeyUp(Keys key)
        {
            if (keys_down.Contains(key))
            {
                keys_down.Remove(key);
            }
        }

        void UpdateKeysDown(HashSet<System.Windows.Forms.Keys> keys)
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
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        public override void OnClosing()
        {
            Keypress.IsConfigOpen = false;
            interceptKeys.OnKeyDown -= InterceptKeys_OnInterceptKeyDown;
            interceptKeys.OnKeyUp -= InterceptKeys_OnInterceptKeyUp;
        }

        public override string Title
        {
            get { return "Key Press"; }
        }

        public override bool OnSave()
        {
            if (KeysToSave.Count == 0)
            {
                System.Windows.MessageBox.Show("You must have at least one key pressed");
                return false;
            }
            return true;
        }

        public override void OnCancel()
        {
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
