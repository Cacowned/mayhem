using System.Windows.Controls;
using WindowModules.Actions;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for WindowMove.xaml
    /// </summary>
    public partial class WindowMinimize : UserControl, WindowActionConfigControl
    {
        WindowActionMinimize action;

        public WindowMinimize(WindowActionMinimize action)
        {
            InitializeComponent();

            this.action = action;
        }

        public void Save()
        {
        }
    }
}
