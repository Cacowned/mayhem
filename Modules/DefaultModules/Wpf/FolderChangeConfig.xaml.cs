using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MayhemWpf.UserControls;

namespace DefaultModules.Wpf
{
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
            CheckName.IsChecked = MonitorName;
            CheckSubdirs.IsChecked = SubDirectories;
            textBoxDirectory.Text = FolderToMonitor;
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.SelectedPath = FolderToMonitor;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxDirectory.Text = dlg.SelectedPath;
            }
        }

        private void Directory_TextChanged(object sender, TextChangedEventArgs e)
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
            else
            {
                FolderToMonitor = textBoxDirectory.Text;
            }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        public override string Title
        {
            get
            {
                return "Folder Change";
            }
        }

        private void CheckName_Checked(object sender, RoutedEventArgs e)
        {
            MonitorName = (bool)CheckName.IsChecked;
        }

        private void CheckSubdirs_Checked(object sender, RoutedEventArgs e)
        {
            SubDirectories = (bool)CheckSubdirs.IsChecked;
        }
    }
}
