using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using Microsoft.Win32;
using System.IO;

namespace DebugModules.Wpf
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AppendLineConfig : WpfConfiguration
    {
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
            FileInfo fileName = new FileInfo(FileBox.Text.Trim());
            FileStream stream = null;

            if (fileName.Exists)
            {
                try
                {
                    stream = fileName.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch (IOException)
                {
                    fileInvalid.Visibility = false ? Visibility.Collapsed : Visibility.Visible;
                    fileOverwrite.Visibility = true ? Visibility.Collapsed : Visibility.Visible;
                    fileNew.Visibility = true ? Visibility.Collapsed : Visibility.Visible;
                    CanSave = false;
                    stream.Close();
                }
                finally
                {
                    if(stream != null)
                    {
                        fileInvalid.Visibility = true ? Visibility.Collapsed : Visibility.Visible;
                        fileOverwrite.Visibility = false ? Visibility.Collapsed : Visibility.Visible;
                        fileNew.Visibility = true ? Visibility.Collapsed : Visibility.Visible;
                        CanSave = true;
                        stream.Close();
                    }
                }
            }

            else
            {
                fileInvalid.Visibility = true ? Visibility.Collapsed : Visibility.Visible;
                fileOverwrite.Visibility = true ? Visibility.Collapsed : Visibility.Visible;
                fileNew.Visibility = false ? Visibility.Collapsed : Visibility.Visible;
                CanSave = true;
            }

            //CanSave = file.Length > 0 && System.IO.File.Exists(FileBox.Text.Trim());
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
