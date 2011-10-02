using System.Windows.Controls;
using WindowModules.Actions;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for WindowMove.xaml
    /// </summary>
    public partial class WindowClose : UserControl, WindowActionConfigControl
    {
        WindowActionClose action;

        public WindowClose(WindowActionClose action)
        {
            InitializeComponent();

            this.action = action;
        }

        public void Save()
        {
        }
    }
}
