using System.Windows.Controls;
using WindowModules.Actions;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for WindowMove.xaml
    /// </summary>
    public partial class WindowMaximize : UserControl, WindowActionConfigControl
    {
        WindowActionMaximize action;

        public WindowMaximize(WindowActionMaximize action)
        {
            InitializeComponent();

            this.action = action;
        }

        public void Save()
        {
        }
    }
}
