using System;
using System.Windows;
using System.Windows.Controls;
using WindowModules.Actions;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for WindowMove.xaml
    /// </summary>
    public partial class WindowResize : UserControl, WindowActionConfigControl
    {
        WindowActionResize action;

        public WindowResize(WindowActionResize action)
        {
            InitializeComponent();

            this.action = action;

            textBoxWidth.Text = action.Width.ToString();
            textBoxHeight.Text = action.Height.ToString();
        }

        public void Save()
        {
            action.Width = int.Parse(textBoxWidth.Text);
            action.Height = int.Parse(textBoxHeight.Text);
        }

        private void buttonSet_Click(object sender, RoutedEventArgs e)
        {
            if (WindowSequenceConfig.CurrentlySelectedWindow != IntPtr.Zero)
            {
                Native.RECT rect = new Native.RECT();
                Native.GetWindowRect(WindowSequenceConfig.CurrentlySelectedWindow, ref rect);
                textBoxWidth.Text = (rect.right - rect.left).ToString();
                textBoxHeight.Text = (rect.bottom - rect.top).ToString();
            }
        }
    }
}
