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
using Microsoft.Win32;

namespace DebugModules.Wpf
{
    /// <summary>
    /// Interaction logic for TimeLogConfig.xaml
    /// </summary>
    public partial class TimeLogConfig : WpfConfiguration
    {
        public string File
        {
            get;
            private set;
        }        
        
        public TimeLogConfig(string file)
        {
            this.File = file;            
            InitializeComponent();
        }

        public override void OnLoad()
        {            
            FileBox.Text = this.File;
        }

        public override void OnSave()
        {
            File = FileBox.Text.Trim();            
        }

        private void CheckFile(string file)
        {
            CanSave = file.Length > 0 && System.IO.File.Exists(FileBox.Text.Trim());
        }

        public override string Title
        {
            get
            {
                return "Time Log";
            }
        }

        private void File_Changed(object sender, TextChangedEventArgs e)
        {
            CheckFile(File);
            fileInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        // Browse for file
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "txt files (*.txt)|*.txt|log files (*.log)|*.log";
            dlg.FileName = File;
            dlg.DefaultExt = ".txt";

            if (dlg.ShowDialog() == true)
            {
                File = dlg.FileName;
                FileBox.Text = File;
            }
        }        
    }
}
