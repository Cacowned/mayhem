using System;
using System.Runtime.Serialization;
using System.Windows.Threading;
using System.Xml;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DefaultModules.Events
{
    [DataContract]
    [MayhemModule("Weather Alert", "This event moniters changes in temperature.")]

    public class WeatherAlert : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string zipCode;

        [DataMember]
        private int temperature;

        [DataMember]
        private bool checkBelow;

        private bool hasPassed;
        private int firstTemp;

        // current temp vs goal temp

        private DispatcherTimer timer;

        protected override void OnLoadDefaults()
        {
            zipCode = "98105";
            temperature = 32;
            checkBelow = true;
        }

        protected override void OnAfterLoad()
        {
            timer = new DispatcherTimer();
            try
            {
                timer.Interval = new TimeSpan(0, 0, 10);
            }
            catch (Exception e)
            {
                Logger.WriteLine(Strings.Timer_CantSetInterval, e.Message);
            }
            timer.Tick += Tick;
        }

        #region Timer
        private void Tick(object sender, EventArgs e)
        {
            checkWeather();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            timer.Start();
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            timer.Stop();
        }
        #endregion

        // add onchanged for weather or trigger in weather function?
        public string GetConfigString()
        {
            string above_below = checkBelow ? "above " : "below ";
            return "Watching " + zipCode + " for " + above_below + temperature + "F";
        }

        private void checkWeather()
        {
            // Retrieve XML document  
            XmlTextReader reader = new XmlTextReader("http://www.google.com/ig/api?weather=" + zipCode.Replace(" ", "%20"));  
            reader.WhitespaceHandling = WhitespaceHandling.Significant;
            
            // Test for internet connection
            try
            {
                reader.Read();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Internet_NotConnected);
            }

            // Read nodes one at a time  
            while (reader.Read())
            {
                if (reader.Name.Equals("temp_f"))
                {
                    int temp = Convert.ToInt32(reader.GetAttribute("data"));
                    if ((checkBelow && temp > temperature) || (!checkBelow && temp < temperature)) 
                    {
                       Trigger();
                    }
                    // close reader after temp is found
                    reader.Close();
                }
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new WeatherAlertConfig(zipCode, temperature, checkBelow); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = (WeatherAlertConfig)configurationControl;
            zipCode = config.ZipCodeProp;
            temperature = config.TempProp;
            checkBelow = config.CheckAbove;
        }
    }
}
