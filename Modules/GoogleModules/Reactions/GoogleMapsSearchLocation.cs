using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using GoogleModules.Resources;
using GoogleModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace GoogleModules.Reactions
{
    /// <summary>
    /// A class that will open a webpage containing the search result on Google Maps for the selected location.
    /// </summary>
    [DataContract]
    [MayhemModule("Google Maps: Search Location", "Searches the selected location")]
    public class GoogleMapsSearchLocation : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string location;

        [DataMember]
        private string mapType;

        private Dictionary<string, string> mapTypesDictionary;

        protected override void OnAfterLoad()
        {
            mapTypesDictionary = new Dictionary<string, string>()
	        {
	            {"Map", "m"},
	            {"Satellite", "h"},
	            {"Terrain", "p"},
	        };
        }

        public override void Perform()
        {
            string url_base = "http://maps.google.com/maps?f=q&hl=en&geocode=&time=&date=&ttype=&q={0}&ie=UTF8&t={1}";
            string requestString = string.Format(url_base, location, mapTypesDictionary[mapType]);

            Process.Start(requestString);
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new GoogleMapsSearchLocationConfig(location, mapType, Strings.GoogleMapsSearchLocation_Title); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as GoogleMapsSearchLocationConfig;

            location = config.Location;
            mapType = config.MapType;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.LocationMapType_ConfigString, location, mapType);
        }

        #endregion
    }
}
