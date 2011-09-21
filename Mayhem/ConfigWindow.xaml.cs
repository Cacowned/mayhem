using System.Threading;
using System.Windows;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using System.Windows.Threading;

namespace Mayhem
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        IWpfConfigurable iWpf;
        IWpfConfiguration iWpfConfig;

        public ConfigWindow(IWpfConfigurable iWpf)
        {
            this.Owner = Application.Current.MainWindow;
            this.iWpf = iWpf;
            this.iWpfConfig = (IWpfConfiguration)iWpf.ConfigurationControl;
            InitializeComponent();
            ConfigContent.Content = iWpfConfig;

            buttonSave.IsEnabled = iWpfConfig.CanSave;

            windowHeader.Text = "Config: " + iWpfConfig.Title;
            iWpfConfig.CanSavedChanged += new IWpfConfiguration.ConfigCanSaveHandler(iWpfConfig_CanSavedChanged);
        }

        void iWpfConfig_CanSavedChanged(bool canSave)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                buttonSave.IsEnabled = canSave;
            }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            iWpfConfig.OnLoad();
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            IWpfConfiguration config = ConfigContent.Content as IWpfConfiguration;
            iWpfConfig.OnSave();
            iWpf.OnSaved(config);
            ((ModuleBase)iWpf).SetConfigString();

            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                iWpfConfig.OnClosing();
            }));
            ((MainWindow)Application.Current.MainWindow).Save();
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                iWpfConfig.OnCancel();
                iWpfConfig.OnClosing();
            }));
            DialogResult = false;
        }
    }
}
