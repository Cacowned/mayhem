using System.Windows;
using System.Windows.Controls;
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

        private void CheckFile(string file)
        {
            CanSave = file.Length > 0 && System.IO.File.Exists(FileBox.Text.Trim());
        }

        public override string Title
        {
            get
            {
                return "Append Line";
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
            dlg.Filter = "txt files (*.txt)|*.txt";
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
