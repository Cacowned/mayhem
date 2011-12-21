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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AppendLineConfig : WpfConfiguration
    {
        private bool shouldCheckVisibility = false;
        public string Line
        {
            get;
            private set;
        }

        public string File
        {
            get;
            private set;
        }

        public AppendLineConfig(string line, string file)
        {
            this.Line = line;
            this.File = file;
            InitializeComponent();
        }

        public override void OnLoad()
        {
            LineBox.Text = this.Line;
            FileBox.Text = this.File;
        }

        public override void OnSave()
        {
            File = FileBox.Text.Trim();
            Line = LineBox.Text.Trim();
        }

        private void checkFile(string File)
        {
            CanSave = File.Length > 0 && System.IO.File.Exists(FileBox.Text.Trim());
        }

        public override string Title
        {
            get
            {
                return "Add Line";
            }
        }

        private void File_Changed(object sender, TextChangedEventArgs e)
        {
            checkFile(File);
            fileInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        // Browse for file
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();

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
