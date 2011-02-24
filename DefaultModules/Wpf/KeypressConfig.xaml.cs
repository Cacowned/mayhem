using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using DefaultModules.KeypressHelpers;
using System.Diagnostics;

namespace DefaultModules.Wpf
{
    public partial class KeypressConfig : Window
    {


        public string KeysDown
        {
            get { return (string)GetValue(KeysDownProperty); }
            set { SetValue(KeysDownProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeysDown.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeysDownProperty =
            DependencyProperty.Register("KeysDown", typeof(string), typeof(KeypressConfig), new UIPropertyMetadata(string.Empty));

        public HashSet<Keys> keys_down = new HashSet<Keys>();

        private InterceptKeys.KeyDownHandler keyDownHandler = null;

        public KeypressConfig()
        {

            InitializeComponent();

            keys_down.Clear();

            keyDownHandler = new InterceptKeys.KeyDownHandler(InterceptKeys_OnInterceptKeyDown);

            InterceptKeys.OnInterceptKeyDown += keyDownHandler;
        }

        void InterceptKeys_OnInterceptKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!keys_down.Contains(e.KeyCode))
            {
                keys_down.Add(e.KeyCode);

                if (keys_down.Count == 1)
                {
                    KeysDown = e.KeyCode.ToString();
                }
                else
                {
                    KeysDown += " + "+e.KeyCode.ToString();
                }
            }
        }

        /**<summary>
         * 
         * Used to add the keypress representation to the widget during de-serialization
         * 
         * </summary>
         */
        public void Deserialize_AddKey(System.Windows.Forms.Keys k)
        {
            if (KeysDown.Length == 0)
            {
                KeysDown = k.ToString();
            }
            else
            {
                KeysDown += " + " + k.ToString();
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (keys_down.Count == 0)
            {
                System.Windows.MessageBox.Show("You must have at least one key pressed");
                return;
            }

            InterceptKeys.OnInterceptKeyDown -= keyDownHandler;

            keyDownHandler = null;
            
            DialogResult = true;
        }

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            KeysDown = "";

            keys_down.Clear();

            if (keyDownHandler == null)
            {
                keyDownHandler = new InterceptKeys.KeyDownHandler(InterceptKeys_OnInterceptKeyDown);
                InterceptKeys.OnInterceptKeyDown += keyDownHandler;
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            InterceptKeys.OnInterceptKeyDown -= keyDownHandler;
        }
    }
}
