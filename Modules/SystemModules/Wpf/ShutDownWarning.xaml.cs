using MayhemWpf.UserControls;

namespace SystemModules.Wpf
{
    /// <summary>
    /// Interaction logic for ShutDownWarning.xaml
    /// </summary>
    public partial class ShutDownWarning : WpfConfiguration
    {
        public bool _forceShutDown 
        { 
            get;
            private set;
        }
        public ShutDownWarning(bool forceShutDown = false)
        {
            InitializeComponent();
            CanSave = true;
            this._forceShutDown = forceShutDown;
        }
        public override string Title
        {
            get{return ("Warning");}
        }
        public override void OnLoad()
        {
            WarningBox.Text = SystemModules.Resources.Strings.Shutdown_ForceWarning;
            EnableForceCheckBox.IsChecked = _forceShutDown;
        }
        public override void OnSave()
        {
            this._forceShutDown = (bool)EnableForceCheckBox.IsChecked;
        }
    }
}
