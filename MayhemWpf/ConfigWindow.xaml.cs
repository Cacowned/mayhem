using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MayhemDefaultStyles.UserControls;
using MayhemCore.ModuleTypes;

namespace MayhemWpf
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        IWpf iWpf;
        IWpfConfig iWpfConfig;

        public ConfigWindow(IWpf iWpf)
        {
            this.Owner = Application.Current.MainWindow;
            this.iWpf = iWpf;
            this.iWpfConfig = (IWpfConfig)iWpf.ConfigurationControl;
            InitializeComponent();
            ConfigContent.Content = iWpfConfig;

            buttonCanSave.IsEnabled = iWpfConfig.CanSave;

            windowHeader.Text = "Config: " + iWpfConfig.Title;
            iWpfConfig.CanSavedChanged += new IWpfConfig.ConfigCanSaveHandler(iWpfConfig_CanSavedChanged);
        }

        void iWpfConfig_CanSavedChanged(bool canSave)
        {
            buttonCanSave.IsEnabled = canSave;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            iWpfConfig.OnLoad();
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (iWpfConfig.OnSave())
            {
                iWpf.OnSaved(ConfigContent.Content as UserControl);
            }
            iWpfConfig.OnClosing();
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            iWpfConfig.OnCancel();
            iWpfConfig.OnClosing();
            DialogResult = false;
        }
    }
}
