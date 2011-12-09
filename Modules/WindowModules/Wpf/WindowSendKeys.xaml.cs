using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowModules.Actions;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for WindowMove.xaml
    /// </summary>
    public partial class WindowSendKeys : UserControl, IWindowActionConfigControl
    {
        private readonly WindowActionSendKeys action;
        private readonly InterceptKeys interceptKeys;

        private readonly List<System.Windows.Forms.Keys> keysDown;

        public WindowSendKeys(WindowActionSendKeys action)
        {
            InitializeComponent();

            this.action = action;

            interceptKeys = InterceptKeys.Instance;

            keysDown = new List<System.Windows.Forms.Keys>();
            foreach (System.Windows.Forms.Keys key in action.KeyList)
            {
                keysDown.Add(key);
            }

            UpdateText();
        }

        private void interceptKeys_OnKeyDown(System.Windows.Forms.Keys key)
        {
            if (key == System.Windows.Forms.Keys.LControlKey || key == System.Windows.Forms.Keys.RControlKey)
                key = System.Windows.Forms.Keys.Control;
            else if (key == System.Windows.Forms.Keys.LMenu || key == System.Windows.Forms.Keys.RMenu || key == System.Windows.Forms.Keys.Menu)
                key = System.Windows.Forms.Keys.Alt;
            else if (key == System.Windows.Forms.Keys.LShiftKey || key == System.Windows.Forms.Keys.RShiftKey || key == System.Windows.Forms.Keys.ShiftKey)
                key = System.Windows.Forms.Keys.Shift;
            if (!keysDown.Contains(key))
            {
                if (key == System.Windows.Forms.Keys.Control || key == System.Windows.Forms.Keys.Shift || key == System.Windows.Forms.Keys.Alt)
                    keysDown.Insert(0, key);
                else
                    keysDown.Add(key);

                UpdateText();
            }
        }

        private void UpdateText()
        {
            string str = string.Empty;
            foreach (System.Windows.Forms.Keys key in keysDown)
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
            textBlockKeys.Text = str;
        }

        public void Save()
        {
            action.KeyList = keysDown;
            interceptKeys.RemoveHook();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            keysDown.Clear();
            UpdateText();
        }

        private void textBlockKeys_GotFocus(object sender, RoutedEventArgs e)
        {
            interceptKeys.OnKeyDown += interceptKeys_OnKeyDown;
            interceptKeys.SetHook();
        }

        private void textBlockKeys_LostFocus(object sender, RoutedEventArgs e)
        {
            interceptKeys.RemoveHook();
            interceptKeys.OnKeyDown -= interceptKeys_OnKeyDown;
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            textBlockKeys_LostFocus(sender, e);
        }

        private void textBlockKeys_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}
