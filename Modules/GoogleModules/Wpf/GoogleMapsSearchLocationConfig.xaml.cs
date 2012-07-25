using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using GoogleModules.Resources;
using MayhemWpf.UserControls;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// User Control for setting the location a user wants to search.
    /// </summary>
    public partial class GoogleMapsSearchLocationConfig : WpfConfiguration
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

        public override string Title
        {
            get { return configTitle; }
        }
        
        private string configTitle;
        private ObservableCollection<string> mapTypes = null;
    
        public GoogleMapsSearchLocationConfig(string locationText, string mapType, string title)
        {
            InitializeComponent();

            Location = locationText;
            MapType = mapType;
            configTitle = title;
        }

        public override void OnLoad()
        {
            CanSave = true;

            LocationBox.Text = Location;
            
            if (mapTypes == null)
                mapTypes = new ObservableCollection<string>();
            else
                mapTypes.Clear();

            mapTypes.Add("Map");
            mapTypes.Add("Satellite");
            mapTypes.Add("Terrain");

            MapTypesComboBox.ItemsSource = mapTypes;

            if (string.IsNullOrEmpty(MapType))
                MapTypesComboBox.SelectedIndex = 0;
            else
                MapTypesComboBox.SelectedItem = MapType;

            DisplayErrorMessage(CheckLocationText());
        }

        public override void OnSave()
        {
            Location = LocationBox.Text;
            MapType = MapTypesComboBox.SelectedItem as string;
        }

        private string CheckLocationText()
        {
            int textLength = LocationBox.Text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
            {
                errorString = Strings.GoogleMaps_Location_NoCharacter;
            }
            else
            {
                if (textLength > 200)
                {
                    errorString = Strings.GoogleMaps_Location_TooLong;
                }
            }

            CanSave = textLength > 0 && (textLength <= 200);

            return errorString;
        }

        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayErrorMessage(CheckLocationText());
        }

        private void DisplayErrorMessage(string errorString)
        {
            // In the case that we have an error message we display it.
            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
            }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
