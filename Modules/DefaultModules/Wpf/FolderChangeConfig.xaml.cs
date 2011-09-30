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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MayhemWpf.UserControls;
using System.IO;
using System.Windows.Forms;

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
            //private set;
        }

        public bool MonitorName
        {
            get
            {
                return (bool)chk_name.IsChecked;
            }
           // private set; 
        }

        public string FolderToMonitor
        {
            get;
            private set; 
        }

        private bool monitorWrite, monitorAccess, monitorName; 

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
            // set the checkbox to be checked by default
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

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
