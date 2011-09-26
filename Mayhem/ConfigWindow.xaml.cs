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
        WpfConfiguration iWpfConfig;

        public ConfigWindow(IWpfConfigurable iWpf)
        {
            this.Owner = Application.Current.MainWindow;
            this.iWpf = iWpf;
            this.iWpfConfig = (WpfConfiguration)iWpf.ConfigurationControl;
            InitializeComponent();
            ConfigContent.Content = iWpfConfig;

            buttonSave.IsEnabled = iWpfConfig.CanSave;

            windowHeader.Text = iWpfConfig.Title;
            iWpfConfig.CanSavedChanged += new WpfConfiguration.ConfigCanSaveHandler(iWpfConfig_CanSavedChanged);
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
            try
            {
                iWpfConfig.OnLoad();
            }
            catch
            {
                MessageBox.Show("Error loading " + iWpfConfig.Name, "Mayhem: Error", MessageBoxButton.OK);
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            WpfConfiguration config = ConfigContent.Content as WpfConfiguration;
            try
            {
                iWpfConfig.OnSave();
                iWpf.OnSaved(config);
                ((ModuleBase)iWpf).SetConfigString();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Error saving " + iWpfConfig.Name);
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                try
                {
                    iWpfConfig.OnClosing();
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, "Error closing " + iWpfConfig.Name + "'s configuration");
                }
            }));
            ((MainWindow)Application.Current.MainWindow).Save();
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                try
                {
                    iWpfConfig.OnCancel();
                    iWpfConfig.OnClosing();
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, "Error cancelling " + iWpfConfig.Name + "'s configuration");
                }
            }));
            DialogResult = false;
        }
    }
}
