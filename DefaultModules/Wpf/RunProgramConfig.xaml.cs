using System.IO;
using System.Windows;
using System.Windows.Controls;
using MayhemDefaultStyles.UserControls;
using Microsoft.Win32;

namespace DefaultModules.Wpf
{
    public partial class RunProgramConfig : IWpfConfiguration
    {
        public string Filename;
        public string Arguments;

        public RunProgramConfig(string filename, string arguments)
        {
            InitializeComponent();
            LocationBox.Text = filename;
            ArgumentsBox.Text = arguments;
            CanSave = File.Exists(filename);
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

        public override string Title
        {
            get { return "Run Program"; }
        }

        public override bool OnSave()
        {
            return true;
        }

        public override void OnCancel()
        {
        }

        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanSave = File.Exists(LocationBox.Text);
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
