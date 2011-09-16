using System.IO;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using Microsoft.Win32;

namespace DefaultModules.Wpf
{
    public partial class PlaySoundConfig : IWpfConfiguration
    {
        public string FileName;

        private bool shouldCheckValidity = false;

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

            shouldCheckValidity = true;
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
                FileName = dlg.FileName;
                LocationBox.Text = FileName;
            }
        }

        private bool CheckValidity()
        {
            CanSave = File.Exists(FileName);

            return CanSave;
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
