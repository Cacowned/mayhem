using System;
using System.Runtime.Serialization;
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
    [MayhemModule("RSS Feed Alert", "Triggers when feed updates")]
    internal class FeedAlert : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string feedUrl;

        private bool hasPassed;
        private DispatcherTimer timer;
        private bool internetFlag;

        public WpfConfiguration ConfigurationControl
        {
            get { return new FeedAlertConfig(); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = (FeedAlertConfig)configurationControl;
            /*
            zipCode = config.ZipCodeProp;
            temperature = config.TempProp;
            checkBelow = config.CheckAbove;
             * */
        }

        public string GetConfigString()
        {
            return "configuring the feeds of this world... alert";
        }

        protected override void OnLoadDefaults()
        {
            feedUrl = "http://feeds.nytimes.com/nyt/rss/HomePage";
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
            if (!Utilities.ConnectedToInternet())
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
            if (Utilities.ConnectedToInternet())
            {
                internetFlag = true;
                try
                {
                    // Retrieve XML document  
                    using (XmlReader reader = new XmlTextReader(feedUrl))
                    {
                        /*
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
                         */
                    }
                }
                catch { }
            }
            else if (internetFlag)
            {
                internetFlag = false;
                ErrorLog.AddError(ErrorType.Warning, Strings.Internet_NotConnected);
            }
        }
    }
}