using System.IO;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using Microsoft.Win32;

namespace DefaultModules.Wpf
{
    public partial class PlaySoundConfig : WpfConfiguration
    {
        public string FileName
        {
            get;
            private set;
        }

        public PlaySoundConfig(string filename)
        {
            this.FileName = filename;
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Play Sound"; }
        }

        public override void OnLoad()
        {
            LocationBox.Text = FileName;
        }

        public override void OnSave()
        {
            FileName = LocationBox.Text;
        }

        // Browse for file
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckPathExists = true;
            dlg.Filter = "All Supported Audio | *.mp3; *.wma; *.wav | MP3s | *.mp3 | WMAs | *.wma | WAVs | *.wav";
            dlg.DefaultExt = ".mp3";
            dlg.FileName = FileName;

            if (dlg.ShowDialog() == true)
            {
                LocationBox.Text = FileName;
            }
        }

        private void CheckValidity()
        {
            CanSave = File.Exists(FileName);
        }

        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
