using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MayhemWpf.UserControls;

namespace DefaultModules.Wpf
{
    /// <summary>
    /// Interaction logic for FolderChangeConfig.xaml
    /// </summary>
    public partial class FolderChangeConfig : WpfConfiguration
    {

    
        public bool MonitorWrite
        {
            get
            {
                return (bool)chk_write.IsChecked;
            }
        }

        public bool MonitorName
        {
            get
            {
                return (bool)chk_name.IsChecked;
            }
        }

        public string FolderToMonitor
        {
            get;
            private set; 
        }

        private bool monitorWrite, monitorName; 

        public FolderChangeConfig(string path, bool mWrite, bool mName)
        {
            InitializeComponent();
            monitorWrite = mWrite;
            monitorName = mName;
            FolderToMonitor = path;
        }

        /// <summary>
        /// Called after the UI has been loaded
        /// </summary>
        public override void OnLoad()
        {
            chk_write.IsChecked = monitorWrite;
            chk_name.IsChecked = monitorName;
            textBoxDirectory.Text = FolderToMonitor;
        }

        // Browse for file
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.RootFolder = Environment.SpecialFolder.Personal;
            dlg.ShowNewFolderButton = true;
            dlg.SelectedPath = FolderToMonitor;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FolderToMonitor = dlg.SelectedPath;
                textBoxDirectory.Text = FolderToMonitor;
            }
        }

        private void textBoxDirectory_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void CheckValidity()
        {
            CanSave = true;
            if (!(textBoxDirectory.Text.Length > 0 && Directory.Exists(textBoxDirectory.Text)))
            {
                textInvalid.Text = "Invalid folder location";
                CanSave = false;
            }       
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        public override string Title
        {
            get
            {
                //return base.Title;
                return "Folder Change";
            }
        }
    }
}
