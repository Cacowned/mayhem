using System.Windows;
using System.Windows.Controls;
using MayhemDefaultStyles.UserControls;
using Microsoft.Win32;
using System.IO;

namespace DefaultModules.Wpf
{
    public partial class PlaySoundConfig : IWpfConfiguration
    {
        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(PlaySoundConfig), new UIPropertyMetadata(null));

        
        public PlaySoundConfig(string filename)
        {
            InitializeComponent();
            this.DataContext = this;

            this.FileName = filename;
            CanSave = File.Exists(FileName);
            textInvalid.Visibility = Visibility.Collapsed;
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
            }
        }

        public override string Title
        {
            get { return "Play Sound"; }
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
            CanSave = File.Exists(FileName);
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
