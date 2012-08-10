using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace OfficeModules.Wpf
{
    public partial class LyncUpdateLocationConfig : WpfConfiguration
    {
        public string Location
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return "Location name"; }
        }

        public LyncUpdateLocationConfig(string location)
        {
            Location = location;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            LocationBox.Text = Location;

            CheckValidity();
        }

        public override void OnSave()
        {
            Location = LocationBox.Text;
        }

        private void CheckValidity()
        {
            CanSave = LocationBox.Text.Trim().Length > 0;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
