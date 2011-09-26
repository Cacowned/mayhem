using System.IO;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using Microsoft.Win32;

namespace DefaultModules.Wpf
{
    public partial class RunProgramConfig : WpfConfiguration
    {
        public string Filename, Arguments;

        private bool shouldCheckValidity = false;

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

            shouldCheckValidity = true;
        }

        private void CheckValidity()
        {
            Filename = LocationBox.Text;
            Arguments = ArgumentsBox.Text;

            CanSave = Filename.Length > 0 && File.Exists(Filename);
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
            if (shouldCheckValidity)
            {
                CheckValidity();

                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
