using MayhemWpf.UserControls;

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
            WarningBox.Text = SystemModules.Resources.Strings.Shutdown_ForceWarning;
            EnableForceCheckBox.IsChecked = ForceShutDown;
        }
        public override void OnSave()
        {
            this.ForceShutDown = (bool)EnableForceCheckBox.IsChecked;
        }
    }
}
