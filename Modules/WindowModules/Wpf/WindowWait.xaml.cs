using System.Windows.Controls;
using WindowModules.Actions;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for WindowMove.xaml
    /// </summary>
    public partial class WindowWait : UserControl, IWindowActionConfigControl
    {
        WindowActionWait action;

        public WindowWait(WindowActionWait action)
        {
            InitializeComponent();

            this.action = action;

            textPauseMs.Text = action.Milliseconds.ToString();
        }

        public void Save()
        {
            action.Milliseconds = int.Parse(textPauseMs.Text);
        }
    }
}
