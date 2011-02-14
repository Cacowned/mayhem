using System.Windows;
using System.Windows.Controls;
using MayhemCore;
using MayhemCore.ModuleTypes;

namespace MayhemWpf.UserControls
{
    /// <summary>
    /// Interaction logic for ModuleView.xaml
    /// </summary>
    public partial class ModuleView : UserControl
    {

        public ModuleBase Module {
            get { return (ModuleBase)GetValue(ModuleProperty); }
            set { SetValue(ModuleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Module.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModuleProperty =
            DependencyProperty.Register("Module", typeof(ModuleBase), typeof(ModuleView), new UIPropertyMetadata(null));

        public ModuleView() {
            InitializeComponent();
        }

        private void ConfigureButton_Click(object sender, RoutedEventArgs e) {
            MainWindow.DimMainWindow(true);

            bool wasEnabled = Module.connection.Enabled;

            Module.connection.Disable();
            ((IWpf)Module).WpfConfig();

            if (wasEnabled)
            {
                Module.connection.Enable();
            }
            /*
            var config = new TimerConfig();
            config.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            config.ShowDialog();
            */
            MainWindow.DimMainWindow(false);
        }
    }
}
