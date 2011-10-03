using System.Windows.Controls;
using WindowModules.Actions;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for WindowMove.xaml
    /// </summary>
    public partial class WindowBringToFront : UserControl, IWindowActionConfigControl
    {
        WindowActionBringToFront action;

        public WindowBringToFront(WindowActionBringToFront action)
        {
            InitializeComponent();

            this.action = action;
        }

        public void Save()
        {
        }
    }
}
