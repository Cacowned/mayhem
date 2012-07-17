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
    [DataContract]
    [MayhemModule("Google Maps: Search Location", "Searches the selected location")]
    public class GoogleMapsSearchLocation : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string location;

        [DataMember]
        private string type;

        public override void Perform()
        {
            string shortType = "h";
            switch (type)
            {
                case "Map":
                    shortType = "m";
                    break;
                case "Satellite":
                    shortType = "h";
                    break;
                case "Terrain":
                    shortType = "p";
                    break;
            }

            string url_base = "http://maps.google.com/maps?f=q&hl=en&geocode=&time=&date=&ttype=&q={0}&ie=UTF8&t={1}";
            string requestString = string.Format(url_base, location, shortType);

            Process.Start(requestString);
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new GoogleMapsSearchLocationConfig(location, type, Strings.GoogleMapsSearchLocation_Title); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as GoogleMapsSearchLocationConfig;

            if (config == null)
            {
                return;
            }

            location = config.Location;
            type = config.MapType;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.LocationMapType_ConfigString, location, type);
        }

        #endregion
    }
}
