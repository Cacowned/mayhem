using System.Windows.Controls;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for WindowMove.xaml
    /// </summary>
    public partial class WindowBringToFront : UserControl, IWindowActionConfigControl
    {
        public WindowBringToFront()
        {
            InitializeComponent();
        }

        public void Save()
        {
        }
    }
}
