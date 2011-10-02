using System;
using System.Windows;
using System.Windows.Controls;
using WindowModules.Actions;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for WindowMove.xaml
    /// </summary>
    public partial class WindowMove : UserControl, WindowActionConfigControl
    {
        WindowActionMove action;

        public WindowMove(WindowActionMove action)
        {
            InitializeComponent();

            this.action = action;

            textBoxMoveX.Text = action.X.ToString();
            textBoxMoveY.Text = action.Y.ToString();
        }

        public void Save()
        {
            action.X = int.Parse(textBoxMoveX.Text);
            action.Y = int.Parse(textBoxMoveY.Text);
        }

        private void buttonSet_Click(object sender, RoutedEventArgs e)
        {
            if (WindowSequenceConfig.CurrentlySelectedWindow != IntPtr.Zero)
            {
                Native.RECT rect = new Native.RECT();
                Native.GetWindowRect(WindowSequenceConfig.CurrentlySelectedWindow, ref rect);
                textBoxMoveX.Text = rect.left.ToString();
                textBoxMoveY.Text = rect.top.ToString();
            }
        }
    }
}
