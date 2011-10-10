using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MayhemWpf.UserControls;
using System.Collections.Generic;

namespace DefaultModules.Wpf
{
    /// <summary>
    /// Interaction logic for FolderChangeConfig.xaml
    /// </summary>
    public partial class FolderChangeConfig : WpfConfiguration
    {

        public bool SubDirectories
        {
            get;
            private set; 
        }
        
        public bool MonitorName
        {
            get;
            private set; 
        }

        public string FolderToMonitor
        {
            get;
            private set; 
        }

        public FolderChangeConfig(string path, bool monitorName, bool subDirs)
        {
            InitializeComponent();
            FolderToMonitor = path;
            MonitorName = monitorName;
            SubDirectories = subDirs; 
        }

        /// <summary>
        /// Called after the UI has been loaded
        /// </summary>
        public override void OnLoad()
        {

            chk_name.IsChecked = MonitorName;
            chk_subdirs.IsChecked = SubDirectories;
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

        private void chk_name_Checked(object sender, RoutedEventArgs e)
        {
            MonitorName = (bool) chk_name.IsChecked;
        }

        private void chk_subdirs_Checked(object sender, RoutedEventArgs e)
        {
            SubDirectories = (bool) chk_subdirs.IsChecked;
        }
    }
}
