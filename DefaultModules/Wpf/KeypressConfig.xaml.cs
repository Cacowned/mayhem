using System.Collections.Generic;
using System.Windows;
using DefaultModules.KeypressHelpers;
using MayhemCore.ModuleTypes;
using System.Windows.Controls;
using System.Diagnostics;
using MayhemDefaultStyles.UserControls;

namespace DefaultModules.Wpf
{
    public partial class KeypressConfig : IWpfConfig
    {
        public HashSet<System.Windows.Forms.Keys> KeysToSave;
        private HashSet<System.Windows.Forms.Keys> keys_down = new HashSet<System.Windows.Forms.Keys>();

        public KeypressConfig(HashSet<System.Windows.Forms.Keys> MonitorKeysDown)
        {
            InitializeComponent();

            this.KeysToSave = MonitorKeysDown;

            InterceptKeys.OnInterceptKeyDown += InterceptKeys_OnInterceptKeyDown;
            InterceptKeys.OnInterceptKeyUp += InterceptKeys_OnInterceptKeyUp;

            UpdateKeysDown(KeysToSave);
        }

        void InterceptKeys_OnInterceptKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!keys_down.Contains(e.KeyCode))
            {
                if (keys_down.Count == 0)
                {
                    KeysToSave.Clear();
                }
                keys_down.Add(e.KeyCode);
                KeysToSave.Add(e.KeyCode);

                UpdateKeysDown(keys_down);
            }
        }

        void InterceptKeys_OnInterceptKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (keys_down.Contains(e.KeyCode))
            {
                keys_down.Remove(e.KeyCode);
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
            Debug.WriteLine(str);
            textBoxKeys.Text = str;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            InterceptKeys.OnInterceptKeyDown -= InterceptKeys_OnInterceptKeyDown;
            InterceptKeys.OnInterceptKeyUp -= InterceptKeys_OnInterceptKeyUp;
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
    }
}
