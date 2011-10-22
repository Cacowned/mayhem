using System.Windows.Controls;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for WindowMove.xaml
    /// </summary>
    public partial class WindowMaximize : UserControl, IWindowActionConfigControl
    {
        public WindowMaximize()
        {
            InitializeComponent();
        }

        public void Save()
        {
        }
    }
}
