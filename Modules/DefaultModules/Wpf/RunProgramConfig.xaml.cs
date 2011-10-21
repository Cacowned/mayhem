using System.IO;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using Microsoft.Win32;

namespace DefaultModules.Wpf
{
    public partial class RunProgramConfig : WpfConfiguration
    {
        public string Filename
        {
            get;
            private set;
        }

        public string Arguments
        {
            get;
            private set;
        }

        public RunProgramConfig(string filename, string arguments)
        {
            this.Filename = filename;
            this.Arguments = arguments;

            InitializeComponent();
        }

        public override string Title
        {
            get { return "Run Program"; }
        }

        public override void OnLoad()
        {
            LocationBox.Text = Filename;
            ArgumentsBox.Text = Arguments;
        }

        public override void OnSave()
        {
            Filename = LocationBox.Text;
            Arguments = ArgumentsBox.Text;
        }

        private void CheckValidity()
        {
            CanSave = LocationBox.Text.Length > 0 && File.Exists(LocationBox.Text);
        }

        // Browse for file
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = Filename;
            dlg.DefaultExt = ".exe";

            if (dlg.ShowDialog() == true)
            {
                Filename = dlg.FileName;
                LocationBox.Text = Filename;
            }
        }

        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
