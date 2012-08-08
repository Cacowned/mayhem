using System.Windows.Controls;
using GoogleModules.Resources;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// User Control for setting the location a user wants to search.
    /// </summary>
    public partial class GoogleMapsSearchLocationConfig : GoogleBaseConfig
    {
        public string Location
        {
            get;
            private set;
        }

        public string MapType
        {
            get;
            private set;
        }

        public GoogleMapsSearchLocationConfig(string locationText, string mapType, string title)
        {
            InitializeComponent();

            Location = locationText;
            MapType = mapType;
            configTitle = title;
        }

        public override void OnLoad()
        {
            LocationBox.Text = Location;

            MapTypesComboBox.Items.Clear();

            MapTypesComboBox.Items.Add("Map");
            MapTypesComboBox.Items.Add("Satellite");
            MapTypesComboBox.Items.Add("Terrain");

            if (string.IsNullOrEmpty(MapType))
            {
                MapTypesComboBox.SelectedIndex = 0;
            }
            else
            {
                MapTypesComboBox.SelectedItem = MapType;
            }

            CheckValidity();
        }

        public override void OnSave()
        {
            Location = LocationBox.Text;
            MapType = MapTypesComboBox.SelectedItem as string;
        }

        protected void CheckValidity()
        {
            CheckValidityField(LocationBox.Text, Strings.GoogleMaps, maxLength: 200);
            DisplayErrorMessage(textInvalid);
        }

        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
