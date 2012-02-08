// This event allows the user to enter a zip code or city name
// and a temperature in deg F. They can choose to select event to 
// Trigger() when the current temperature drops below or rises above
// the user-entered temperature. If the current temperature is already
// below the user-entered temperature, in the case they want the event
// to trigger when the current temperature drops below, it will trigger
// on the first call, and not after that unless the event is reset
// (switched off/on or created anew).

using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml;
using DefaultModules.LowLevel;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DefaultModules.Events
{
    [DataContract]
    [MayhemModule("Weather Alert", "Monitors changes in temperature")]

    public class WeatherAlert : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string zipCode;

        [DataMember]
        private int temperature;

        [DataMember]
        private bool checkBelow;

        private bool hasPassed;
        private bool internetFlag;
        private DispatcherTimer timer;

        // number of degrees temp must re-pass to enable triggering again
        private static int OFFSET = 1;

        // Store all the variables from the configuration dialog into
        // local data members
        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = (WeatherAlertConfig)configurationControl;
            zipCode = config.ZipCodeProp;
            temperature = config.TempProp;
            checkBelow = config.CheckAbove;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new WeatherAlertConfig(zipCode, temperature, checkBelow); }
        }

        // returns "Watching {zip code or city name} for {above or below X}F"
        public string GetConfigString()
        {
            string above_below = checkBelow ? "above " : "below ";
            above_below += temperature;
            return String.Format("Watching {0} for {1}F", zipCode, above_below);
        }

        // Default city of Seattle WA with a temperature of 32F and a bool
        // to trigger when the temperature drops below 32
        protected override void OnLoadDefaults()
        {
            zipCode = "98105";
            temperature = 32;
            checkBelow = true;
        }

        // Initialize timer, reset temperature trigger, links the webRequest to the zipCode
        // entered in the configuration dialog
        protected override void OnAfterLoad()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 2, 0);
            timer.Tick += CheckWeather;
            hasPassed = false;
        }

        // Starts the timer, already set to an interval of 1 minute
        // when the temperature is reached or passed, trigger event once
        #region Timer
        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!Utilities.ConnectedToInternet())
            {
                ErrorLog.AddError(ErrorType.Warning, String.Format(Strings.Internet_NotConnected, "Weather"));
            }
            // when turned off then off again, will check for passing weather point
            hasPassed = false;
            timer.Start();
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            timer.Stop();
        }
        #endregion

        // Grabs the current weather information for the city / zipcode being watched
        private void CheckWeather(object sender, EventArgs e)
        {
            // Test for internet connection
            if (Utilities.ConnectedToInternet())
            {
                internetFlag = true;
                try
                {
                    // get the xml data
                    WebRequest webRequest = WebRequest.Create("http://www.google.com/ig/api?weather=" + zipCode.Replace(" ", "%20"));
                    using (WebResponse webResponse = webRequest.GetResponse())
                    {
                        HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                        webRequest.CachePolicy = noCachePolicy;
                        using (Stream responseStream = webResponse.GetResponseStream())
                        {
                            // Used to crash on "Paris" because of encoding issues, now using 
                            // Western-European Windows encoding
                            StreamReader s = new StreamReader(responseStream, Encoding.GetEncoding(1252));
                            XmlReader r = XmlTextReader.Create(s);

                            r.ReadToFollowing("temp_f");
                            int temp = Convert.ToInt32(r.GetAttribute("data"));

                            bool isBelowOrAbove = (checkBelow && temp >= temperature) || (!checkBelow && temp <= temperature);

                            // if below desired temperature and watching for below, trigger
                            // if above desired temperature and watching for abovem trigger
                            if (isBelowOrAbove)
                            {
                                bool reset = (temp == temperature + OFFSET) && checkBelow || (temp == temperature - OFFSET) && !checkBelow;
                                if (!hasPassed)
                                {
                                    hasPassed = true;
                                    Trigger();
                                }
                                else if (reset)
                                {
                                    hasPassed = false;
                                }
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString());
                }
            }
            else if (internetFlag)
            {
                internetFlag = false;
                ErrorLog.AddError(ErrorType.Warning, String.Format(Strings.Internet_NotConnected, "Weather"));
            }
        }
    }
}
