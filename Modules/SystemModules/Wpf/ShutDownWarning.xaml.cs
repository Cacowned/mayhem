using MayhemWpf.UserControls;
using SystemModules.Resources;

namespace SystemModules.Wpf
{
    /// <summary>
    /// Interaction logic for ShutDownWarning.xaml
    /// </summary>
    public partial class ShutDownWarning : WpfConfiguration
    {
        public bool ForceShutDown 
        { 
            get;
            private set;
        }

        public ShutDownWarning(bool forceShutDown = false)
        {
            InitializeComponent();
            CanSave = true;
            this.ForceShutDown = forceShutDown;
        }

        public override string Title
        {
            get{return ("Warning");}
        }

        public override void OnLoad()
        {
            WarningBox.Text = Strings.Shutdown_ForceWarning;
            EnableForceCheckBox.IsChecked = this.ForceShutDown;
        }

        public override void OnSave()
        {
            this.ForceShutDown = (bool)EnableForceCheckBox.IsChecked;
        }
    }
}
