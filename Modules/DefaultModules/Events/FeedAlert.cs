using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using System.Windows.Threading;
using MayhemWpf.UserControls;
using System.Runtime.InteropServices;
using DefaultModules.Resources;
using System.Xml;
using DefaultModules.Wpf;

namespace DefaultModules.Events
{
    [DataContract]
    [MayhemModule("RSS Feed Alert", "Monitors changes in temperature")]

    public class FeedAlert : EventBase, IWpfConfigurable
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
            var config = (FeedAlertConfig)configurationControl;
            /*
            zipCode = config.ZipCodeProp;
            temperature = config.TempProp;
            checkBelow = config.CheckAbove;
             * */
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new FeedAlertConfig(); }
        }

        public string GetConfigString()
        {
            return "configuring the feeds of this world... alert";
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
            timer.Tick += CheckFeed;
            hasPassed = false;
        }

        #region Timer
        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!ConnectedToInternet())
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.Internet_NotConnected);
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

        private void CheckFeed(object sender, EventArgs e)
        {
            // Test for internet connection
            if (ConnectedToInternet())
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
                ErrorLog.AddError(ErrorType.Warning, Strings.Internet_NotConnected);
            }
        }

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int desciption, int reservedValue);

        private static bool ConnectedToInternet()
        {
            int desc;
            try
            {
                return InternetGetConnectedState(out desc, 0);
            }
            catch
            {
                return false;
            }
        }
    }
}