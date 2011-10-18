using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using DefaultModules.Events;
using DefaultModules.KeypressHelpers;
using MayhemWpf.UserControls;

namespace DefaultModules.Wpf
{
    public partial class KeypressConfig : WpfConfiguration
    {
        // This is the public field Keypress uses to get which keys to use
        public HashSet<Keys> KeysToSave
        {
            get;
            private set;
        }
        
        private HashSet<Keys> keysDown = new HashSet<Keys>();

        // Contains the string the interface binds to
        public string KeysDownText
        {
            get { return (string)GetValue(KeysDownTextProperty); }
            set { SetValue(KeysDownTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeysDownText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeysDownTextProperty =
            DependencyProperty.Register("KeysDownText", typeof(string), typeof(KeypressConfig), new UIPropertyMetadata(string.Empty));

        private bool shouldCheckValidity = false;

        public KeypressConfig(HashSet<Keys> monitorKeysDown)
        {
            KeysToSave = new HashSet<Keys>(monitorKeysDown);

            this.DataContext = this;
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Key Press"; }
        }

        public override void OnLoad()
        {
            KeyPress.IsConfigOpen = true;

            InterceptKeys keys = InterceptKeys.Instance;
            keys.AddRef();
            keys.OnKeyDown += InterceptKeys_OnInterceptKeyDown;
            keys.OnKeyUp += InterceptKeys_OnInterceptKeyUp;

            UpdateKeysDown();

            shouldCheckValidity = true;
        }

        private void InterceptKeys_OnInterceptKeyDown(Keys key)
        {
            if (!keysDown.Contains(key))
            {
                if (keysDown.Count == 0)
                {
                    KeysToSave.Clear();
                }

                keysDown.Add(key);
                KeysToSave.Add(key);

                UpdateKeysDown();
            }
        }

        private void InterceptKeys_OnInterceptKeyUp(Keys key)
        {
            if (keysDown.Contains(key))
            {
                keysDown.Remove(key);
            }
        }

        private void UpdateKeysDown()
        {
            string str = string.Empty;
            foreach (Keys key in KeysToSave)
            {
                if (str == string.Empty)
                {
                    str = key.ToString();
                }
                else
                {
                    str += " + " + key.ToString();
                }
            }

            KeysDownText = str;

            CanSave = KeysToSave.Count > 0;
            if (shouldCheckValidity)
            {
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public override void OnClosing()
        {
            InterceptKeys keys = InterceptKeys.Instance;
            keys.RemoveRef();
            keys.OnKeyDown -= InterceptKeys_OnInterceptKeyDown;
            keys.OnKeyUp -= InterceptKeys_OnInterceptKeyUp;

            KeyPress.IsConfigOpen = false;
        }

        private void WpfConfiguration_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }
    }
}
