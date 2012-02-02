using System;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using MayhemWpf.UserControls;
using System.Runtime.InteropServices;

namespace DefaultModules.Wpf
{
    public partial class FeedAlertConfig : WpfConfiguration
    {

        private DispatcherTimer timer;
        private string feedName;
        private XmlNode channelNode;

        public string UrlProp
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
            //timer.Tick += CheckInternet;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            RSSUrl.Text = UrlProp;
        }

        public override void OnSave()
        {
            UrlProp = RSSUrl.Text;
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
                    using (XmlReader feedData = new XmlTextReader("http://www.npr.org/rss/rss.php?id=1012"))
                    {

                        XmlDocument rssDoc = new XmlDocument();
                        XmlNode rssNode = rssDoc.ChildNodes[0];
                        channelNode = rssDoc.ChildNodes[0];

                        rssDoc.Load(feedData);

                        for (int i = 0; i < rssDoc.ChildNodes.Count; i++)
                        {
                            if (rssDoc.ChildNodes[i].Name == "rss")
                            {
                                rssNode = rssDoc.ChildNodes[i];
                            }
                        }

                        for (int i = 0; i < rssNode.ChildNodes.Count; i++)
                        {
                            if (rssNode.ChildNodes[i].Name == "channel")
                            {
                                channelNode = rssNode.ChildNodes[i];
                            }
                        }

                        for (int i = 0; i < channelNode.ChildNodes.Count; i++)
                        {
                            if (channelNode.ChildNodes[i].Name == "item")
                            {
                                return true;
                            }
                        }

                        return false;
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
                FeedName.Text = "Feed Source";
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
                FeedName.Text = channelNode["generator"].InnerText.ToString();
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
            catch { }

            if (!timer.IsEnabled)
            {
                timer.Start();
            }
            return false;
        }


    }
}
