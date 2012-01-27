using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Threading;
using System.Xml;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using DefaultModules.LowLevel;

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
        private DispatcherTimer timer;
        private bool internetFlag;

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

        public string GetConfigString()
        {
            string above_below = checkBelow ? "above " : "below ";
            above_below += temperature;
            return String.Format("Watching {0} for {1}F", zipCode, above_below);
        }

        protected override void OnLoadDefaults()
        {
            zipCode = "98105";
            temperature = 32;
            checkBelow = true;
        }

        protected override void OnAfterLoad()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 1, 0);
            timer.Tick += CheckWeather;
            hasPassed = false;
        }

        #region Timer
        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!Utilities.ConnectedToInternet())
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.WeatherAlert_Internet_NotConnected);
            }
            timer.Start();
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            timer.Stop();
            // when turned off then off again, will check for passing weather point
            hasPassed = false;
        }
        #endregion

        private void CheckWeather(object sender, EventArgs e)
        {
            // Test for internet connection
            if (Utilities.ConnectedToInternet())
            {
                internetFlag = true;
                // Retrieve XML document  
                XmlTextReader reader = new XmlTextReader("http://www.google.com/ig/api?weather=" + zipCode.Replace(" ", "%20"));
                reader.WhitespaceHandling = WhitespaceHandling.Significant;

                reader.ReadToFollowing("temp_f");
                int temp = Convert.ToInt32(reader.GetAttribute("data"));

                bool isBelowOrAbove = (checkBelow && temp >= temperature) || (!checkBelow && temp <= temperature);

                // if below desired temperature and watching for below, trigger
                // if above desired temperature and watching for abovem trigger
                if (isBelowOrAbove)
                {
                    if (!hasPassed)
                    {
                        hasPassed = true;
                        Trigger();
                    }
                    else if (temp == temperature)
                    {
                        hasPassed = false;
                    }
                }

            }
            else if (internetFlag)
            {
                internetFlag = false;
                ErrorLog.AddError(ErrorType.Warning, Strings.WeatherAlert_Internet_NotConnected);
            }
        }
    }
}
