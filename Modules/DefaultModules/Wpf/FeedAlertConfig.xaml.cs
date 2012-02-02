using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using MayhemWpf.UserControls;

namespace DefaultModules.Wpf
{
    public partial class FeedAlertConfig : WpfConfiguration
    {
        private DispatcherTimer timer;
        private XmlNode channelNode;

        public string UrlProp
        {
            get;
            private set;
        }

        public string FeedTitleProp
        {
            get;
            private set;
        }

        public override string Title
        {
            get
            {
                return "RSS Feed Alert";
            }
        }

        public FeedAlertConfig(string url)
        {
            UrlProp = url;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.Tick += CheckInternet;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            RSSUrl.Text = UrlProp;
        }

        public override void OnSave()
        {
            UrlProp = RSSUrl.Text;
            FeedTitleProp = FeedCatergory.Text;
        }

        private void CheckInternet(object sender, EventArgs e)
        {
            // don't want to add overloads to ConnectedToInternet()
            ConnectedToInternet();
        }

        private void VerifyFields()
        {
            string error = "Invalid";
            error += RSSUrl.Text.Length > 0 && IsValidFeed() ? String.Empty : " feed url";
            CanSave = error.Equals("Invalid");
            TextChanged(error);
        }

        // Verify that the feed is a valid xml rss feed containting a rss, channel, and item tag
        private bool IsValidFeed()
        {
            if (!timer.IsEnabled)
            {
                try
                {
                    string url = RSSUrl.Text;
                    WebRequest webRequest = WebRequest.Create(url);
                    using (WebResponse webResponse = webRequest.GetResponse())
                    {
                        using (Stream responseStream = webResponse.GetResponseStream())
                        {
                            StreamReader s = new StreamReader(responseStream, Encoding.GetEncoding(1252));
                            XmlDocument rssDoc = new XmlDocument();
                            XmlNode rssNode = rssDoc.ChildNodes[0];
                            channelNode = rssDoc.ChildNodes[0];

                            rssDoc.Load(s);

                            for (int i = 0; i < rssDoc.ChildNodes.Count; i++)
                            {
                                if (rssDoc.ChildNodes[i].Name == "rss")
                                {
                                    rssNode = rssDoc.ChildNodes[i];
                                }
                            }

                            if (rssNode != rssDoc.ChildNodes[0])
                            {
                                for (int i = 0; i < rssNode.ChildNodes.Count; i++)
                                {
                                    if (rssNode.ChildNodes[i].Name == "channel")
                                    {
                                        channelNode = rssNode.ChildNodes[i];
                                    }
                                }
                            }

                            if (channelNode != rssDoc.ChildNodes[0])
                            {
                                for (int i = 0; i < channelNode.ChildNodes.Count; i++)
                                {
                                    if (channelNode.ChildNodes[i].Name == "item")
                                    {
                                        return true;
                                    }
                                }
                            }

                            return false;
                        }
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        private void URL_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (ConnectedToInternet())
            {
                VerifyFields();
            }
            else
            {
                TextChanged("Cannot connect to the Internet");
            }
        }

        private void TextChanged(string text)
        {
            textInvalid.Text = text;
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            FeedCatergory.Visibility = CanSave ? Visibility.Visible : Visibility.Collapsed;

            if (CanSave)
            {
                FeedCatergory.Text = channelNode["title"].InnerText.ToString();
            }
        }

        private bool ConnectedToInternet()
        {
            try
            {
                System.Net.IPHostEntry obj = System.Net.Dns.GetHostEntry("www.google.com");
                if (timer.IsEnabled)
                {
                    timer.Stop();
                    VerifyFields();
                }

                return true;
            }
            catch
            {
            }

            if (!timer.IsEnabled)
            {
                timer.Start();
            }

            return false;
        }
    }
}
