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
        /// <summary>
        /// The location that will be searched
        /// </summary>
        public string Location
        {
            get;
            private set;
        }

        /// <summary>
        /// The type of the map that will be displayed
        /// </summary>
        public string MapType
        {
            get;
            private set;
        }

        /// <summary>
        /// The title of the user control.
        /// </summary>
        public override string Title
        {
            get { return configTitle; }
        }
        
        private string configTitle;
        private ObservableCollection<string> mapTypes = null;

        /// <summary>
        /// The constructor of the GoogleMapsSearchLocationConfig class.
        /// </summary>
        /// <param name="locationText">The location that will be searched</param>
        /// <param name="mapType">The type of the map</param>
        /// <param name="title">The title of the user control</param>        
        public GoogleMapsSearchLocationConfig(string locationText, string mapType, string title)
        {
            InitializeComponent();

            Location = locationText;
            MapType = mapType;
            configTitle = title;
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
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

        /// <summary>
        /// This method will be called when the used clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            Location = LocationBox.Text;
            MapType = MapTypesComboBox.SelectedItem as string;
        }

        /// <summary>
        /// This method will check if the location is valid.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
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

        /// <summary>
        /// This method will be called when the text from the LocationBox changes.
        /// </summary>
        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayErrorMessage(CheckLocationText());
        }

        /// <summary>
        /// Displays the error message received as parameter.
        /// </summary>
        /// <param name="errorMessage">The text of the error message</param>
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
