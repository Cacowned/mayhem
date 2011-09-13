using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowModules.Actions;
using System.Diagnostics;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for WindowMove.xaml
    /// </summary>
    public partial class WindowSendKeys : UserControl, WindowActionConfigControl
    {
        WindowActionSendKeys action;
        private InterceptKeys interceptKeys;

        List<Key> keys = new List<Key>();
        private List<System.Windows.Forms.Keys> keys_down = new List<System.Windows.Forms.Keys>();

        public WindowSendKeys(WindowActionSendKeys action)
        {
            InitializeComponent();

            this.action = action;

            interceptKeys = InterceptKeys.Instance;

            foreach (System.Windows.Forms.Keys key in action.KeyList)
            {
                keys_down.Add(key);
            }
            UpdateText();
        }

        void interceptKeys_OnKeyDown(System.Windows.Forms.Keys key)
        {
            if (key == System.Windows.Forms.Keys.LControlKey || key == System.Windows.Forms.Keys.RControlKey)
                key = System.Windows.Forms.Keys.Control;
            else if (key == System.Windows.Forms.Keys.LMenu || key == System.Windows.Forms.Keys.RMenu || key == System.Windows.Forms.Keys.Menu)
                key = System.Windows.Forms.Keys.Alt;
            else if (key == System.Windows.Forms.Keys.LShiftKey || key == System.Windows.Forms.Keys.RShiftKey || key == System.Windows.Forms.Keys.ShiftKey)
                key = System.Windows.Forms.Keys.Shift;
            if (!keys_down.Contains(key))
            {
                if (key == System.Windows.Forms.Keys.Control || key == System.Windows.Forms.Keys.Shift || key == System.Windows.Forms.Keys.Alt)
                    keys_down.Insert(0, key);
                else
                    keys_down.Add(key);

                UpdateText();
            }
        }

        void UpdateText()
        {
            string str = "";
            foreach (System.Windows.Forms.Keys key in keys_down)
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
            textBlockKeys.Text = str;
        }

        public void Save()
        {
            action.KeyList = keys_down;
            interceptKeys.RemoveHook();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            keys_down.Clear();
            UpdateText();
        }

        private void textBlockKeys_GotFocus(object sender, RoutedEventArgs e)
        {
            interceptKeys.OnKeyDown += new InterceptKeys.KeyDownHandler(interceptKeys_OnKeyDown);
            interceptKeys.SetHook();
        }

        private void textBlockKeys_LostFocus(object sender, RoutedEventArgs e)
        {
            interceptKeys.RemoveHook();
            interceptKeys.OnKeyDown -= new InterceptKeys.KeyDownHandler(interceptKeys_OnKeyDown);
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            textBlockKeys_LostFocus(sender, e);
        }

        private void textBlockKeys_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        /*
        private void textBlockKeys_KeyDown(object sender, KeyEventArgs e)
        {
            Key key = e.Key;
            if(key == Key.RightCtrl)
                key = Key.LeftCtrl;
            if(key == Key.RightAlt)
                key = Key.LeftAlt;
            if(key == Key.RightShift)
                key = Key.LeftShift;
            if (key == Key.System)
                key = Key.LeftAlt;
            if (key == Key.RWin)
                key = Key.LWin;

            Debug.WriteLine(key);

            keys.Add(key);
  //          SetText();

            if(key == Key.LeftCtrl || key == Key.LeftAlt)
                e.Handled = true;
        }

        void SetText()
        {
            string text = "";
            for (int i = 0; i < keys.Count; i++)
            {
                string keyString = keys[i].ToString();
                switch(keyString)
                {
                    case "LeftCtrl":
                        keyString = "Ctrl";
                        break;
                    case "LeftShift":
                        keyString = "Shift";
                        break;
                    case "LeftAlt":
                        keyString = "Alt";
                        break;
                    case "LWin":
                        keyString = "Win";
                        break;
                    case "Space":
                        keyString = "";
                        break;
                    case "Capital":
                        keyString = "Caps";
                        break;
                    case "D0":
                    case "D1":
                    case "D2":
                    case "D3":
                    case "D4":
                    case "D5":
                    case "D6":
                    case "D7":
                    case "D8":
                    case "D9":
                        keyString = keyString[1].ToString();
                        break;
                    case "Multiply":
                        keyString = "*";
                        break;
                    case "Add":
                        keyString = "+";
                        break;
                    case "Subtract":
                        keyString = "-";
                        break;
                    case "Decimal":
                        keyString = ".";
                        break;
                    case "Divide":
                        keyString = "/";
                        break;
                    case "Numpad0":
                    case "Numpad1":
                    case "Numpad2":
                    case "Numpad3":
                    case "Numpad4":
                    case "Numpad5":
                    case "Numpad6":
                    case "Numpad7":
                    case "Numpad8":
                    case "Numpad9":
                        keyString = keyString.Replace("Numpad", "");
                        break;
                    case "Oem3":
                        keyString = "~";
                        break;
                }
                text += keyString + " ";
            }
            textBlockKeys.Text = text;
        }
         * */
    }
}
