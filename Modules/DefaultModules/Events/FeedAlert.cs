using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
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

    public class FeedAlert : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string feedUrl;

        [DataMember]
        private string feedTitle;

        // Does not store any data from the configuration
        // Stores the "latest" feed item title to compare
        // against and check for updates
        [DataMember]
        private string feedData;

        private bool hasUpdated;
        private DispatcherTimer timer;

        public WpfConfiguration ConfigurationControl
        {
            get { return new FeedAlertConfig(feedUrl); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = (FeedAlertConfig)configurationControl;
            feedUrl = config.UrlProp;
            feedTitle = config.FeedTitleProp;
        }

        public string GetConfigString()
        {
            return String.Format("Watching {0} feed", feedTitle);
        }

        protected override void OnLoadDefaults()
        {
            feedUrl = "http://feeds.nytimes.com/nyt/rss/HomePage";
            feedData = String.Empty;
        }

        protected override void OnAfterLoad()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 3);
            timer.Tick += CheckFeed;
            hasUpdated = false;
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
            hasUpdated = false;
        }
        #endregion

        private void CheckFeed(object sender, EventArgs e)
        {
            // Test for internet connection
            if (Utilities.ConnectedToInternet())
            {
                try
                {
                    WebRequest webRequest = WebRequest.Create(feedUrl);
                    using (WebResponse webResponse = webRequest.GetResponse())
                    {
                        using (Stream responseStream = webResponse.GetResponseStream())
                        {
                            StreamReader s = new StreamReader(responseStream, Encoding.GetEncoding(1252));
                            XmlDocument rssDoc = new XmlDocument();
                            rssDoc.Load(s);

                            string tempFeedData = GetTitles(rssDoc);

                            if (feedData == String.Empty)
                                feedData = tempFeedData;
                            else if (!tempFeedData.Equals(feedData)) // && !hasUpdated)
                            {
                                // hasUpdated = true;
                                Trigger();
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            else
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.Internet_NotConnected);
            }
        }

        // recursive helper method that parses all Title Values from RSS feed of item elements
        // combined all the titles together because rss some RSS feeds do not update linerally
        // i.e. the second item could update with the first item unchanged
        private string GetTitles(XmlNode node)
        {
            if (node.Name == "item")
                return node.FirstChild.InnerText;
            else
            {
                string builder = String.Empty;
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    if (node.ChildNodes[i].ChildNodes.Count > 0 && node.ChildNodes[i].FirstChild.NodeType != XmlNodeType.Text)
                        builder += GetTitles(node.ChildNodes[i]) + " ";
                }

                return builder;
            }
        }
    }
}