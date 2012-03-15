using System;
using System.IO;
using System.Net;
using System.Net.Cache;
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
    /// <summary>
    /// This module allows a user to enter an URL for a RSS feed
    /// and will trigger when that feed has been updated.
    /// </summary>
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
            // reset state if feed url has changed, don't want it to trigger
            feedData = "";
        }

        /// <summary>
        /// Watching {Tile of feed} feed
        /// </summary>
        public string GetConfigString()
        {
            return String.Format("Watching {0} feed", feedTitle);
        }

        /// <summary>
        /// Default feed URL is the new york times home page
        /// </summary>
        protected override void OnLoadDefaults()
        {
            feedUrl = "http://feeds.nytimes.com/nyt/rss/HomePage";
            feedData = String.Empty;
        }

        /// <summary>
        /// Start the feed checking timer, checks once every two minutes
        /// </summary>
        protected override void OnAfterLoad()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 2, 0);
            timer.Tick += CheckFeed;
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
        }
        #endregion

        /// <summary>
        /// Checks if there are any changes to the RSS feed that is being watched
        /// </summary>
        private void CheckFeed(object sender, EventArgs e)
        {
            // Test for internet connection
            if (Utilities.ConnectedToInternet())
            {
                try
                {
                    WebRequest webRequest = WebRequest.Create(feedUrl);
                    HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                    // otherwise stores xml info in the cache
                    webRequest.CachePolicy = noCachePolicy;
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
                            else if (!tempFeedData.Equals(feedData))
                            {
                                feedData = tempFeedData;
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

        /// <summary>
        /// recursive helper method that parses all Title Values from RSS feed of item elements
        /// combined all the titles together because rss some RSS feeds do not update linerally
        /// i.e. the second item could update with the first item unchanged
        /// </summary>
        /// <param name="node">The xml document to get all title tags of</param> 
        /// <returns>the contents of the tile elements as a string</returns>
        private string GetTitles(XmlNode node)
        {
            if (node.Name == "item")
                return node.FirstChild.InnerText;
        	
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