﻿using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using MayhemWpf.UserControls;

namespace DefaultModules.Wpf
{
    /// <summary>
    /// This is the configuration file for the FeedAlert event
    /// The event is explained in FeedAlert.cs, the configuration
    /// file checks for a valid RSS feed URL.
    /// </summary>
    public partial class FeedAlertConfig : WpfConfiguration
    {
        private DispatcherTimer timer;
        private XmlNode channelNode;
        private bool doneLoading;

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

        public bool RequireProxy
        {
            get;
            private set;
        }

        public string[] ProxySettingsProp
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

        /// <param name="url">RSS URL as a string</param>
        public FeedAlertConfig(string url, bool proxy, string[] proxySettings)
        {
            UrlProp = url;
            RequireProxy = proxy;
            ProxySettingsProp = proxySettings;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.Tick += CheckInternet;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            RSSUrl.Text = UrlProp;
            Proxy.IsChecked = RequireProxy;
            doneLoading = false;

            if (RequireProxy)
            {
                User.Text = ProxySettingsProp[0];
                Pass.Password = ProxySettingsProp[1];
                Domain.Text = ProxySettingsProp[2];
            }
            else
            {
                ProxyGrid.Visibility = Visibility.Collapsed;
            }
            doneLoading = true;

        }

        public override void OnSave()
        {
            UrlProp = RSSUrl.Text;
            FeedTitleProp = FeedCatergory.Text;

            // if require proxy is not checked, clear user, pass, domain
            // otherwise the fields are updated when the text is changed
            if (!RequireProxy)
            {
                ProxySettingsProp = new string[3];
            }
        }

        /// <summary>
        /// Didn't want to add overloads to ConnectedToInternet()
        /// </summary>
        private void CheckInternet(object sender, EventArgs e)
        {
            ConnectedToInternet();
        }

        /// <summary>
        /// Veryify the that RSS URL field is longer than 0, calls IsValidFeed
        /// </summary>
        private void VerifyFields()
        {
            string error = "Invalid";
            error += RSSUrl.Text.Length > 0 && IsValidFeed() ? String.Empty : " feed url";
            CanSave = error.Equals("Invalid");
            TextChanged(error);
        }

        /// <summary>
        /// Verify that the feed is a valid xml rss feed containting a rss, channel, and item tag
        /// </summary>
        /// <returns>true if the feed is valid, false if not</returns>
        private bool IsValidFeed()
        {
            if (!timer.IsEnabled)
            {
                try
                {
                    string url = RSSUrl.Text;
                    WebRequest webRequest = WebRequest.Create(url);

                    // If the server needs a proxy
                    // TODO: ADD TO CS FILE
                    if (RequireProxy)
                    {
                        webRequest.Proxy = WebRequest.DefaultWebProxy;
                        // USER, PASS, DOMAIN
                        webRequest.Credentials = new NetworkCredential(ProxySettingsProp[0], ProxySettingsProp[1], ProxySettingsProp[2]);
                        webRequest.Proxy.Credentials = new NetworkCredential(ProxySettingsProp[0], ProxySettingsProp[1], ProxySettingsProp[2]);
                    }

                    // set timeout to 600ms
                    webRequest.Timeout = 600;
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
                                        // Have found all thee RSS, channel, and item tags
                                        return true;
                                    }
                                }
                            }

                            return false;
                        }
                    }
                }
                catch (Exception e)
                {
                    TextChanged("Invalid URL");
                }
            }

            return false;
        }

        /// <summary>
        /// Triggered when the RSS URL text field is changed, if there is no Internet connection
        /// display "Cannot connect to the Internet"
        /// </summary>
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

        /// <summary>
        /// Shows / hides the error message dependant on wether the RSS URL is valid
        /// if valid, grabs the title of the RSS and displays it
        /// </summary>
        private void TextChanged(string text)
        {
            textInvalid.Text = text;
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            //FeedCatergory.Visibility = CanSave ? Visibility.Visible : Visibility.Collapsed;

            if (CanSave)
            {
                FeedCatergory.Text = channelNode["title"].InnerText;
            }
        }

        /// <summary>
        /// Checks for an Internet connection
        /// </summary>
        /// <returns>true if there is a current Internet connection</returns>
        #region CheckInternet
        private bool ConnectedToInternet()
        {
            try
            {
                // check Internet connection (ping google w/ 650ms timeout
                CallWithTimeout(MakeRequest, 650);
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

        /// <summary>
        /// Make a request to google.com
        /// </summary>
        private static void MakeRequest()
        {
            IPHostEntry obj = Dns.GetHostEntry("www.google.com");
        }

        /// <summary>
        /// A method that will trigger a "timeout" error to be used when running a method with a 
        /// specific timout trigger
        /// </summary>
        /// <param name="action">The method being run</param>
        /// <param name="timeout">The timeout in milliseconds</param>
        public static void CallWithTimeout(Action action, int timeout)
        {
            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                action();
            };

            IAsyncResult result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeout))
            {
                wrappedAction.EndInvoke(result);
                //throw new TimeoutException();
            }
            else
            {
                threadToKill.Abort();
                throw new TimeoutException();
            }
        }
        #endregion

        private void proxy_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string error = string.Empty;
            if (User.Text.Equals(string.Empty))
            {
                error += "Username must not be blank. ";
            } 
            if (Domain.Text.Equals(string.Empty))
            {
                error += "Domain must not be blank.";
            }

            if(!error.Equals(string.Empty))
            {
                CanSave = false;
                TextChanged(error);
            } else // if other fields are good CanSave will be set to true
            {
                VerifyFields();
            }


            if (doneLoading)
            {
                ProxySettingsProp[0] = User.Text;
                ProxySettingsProp[1] = Pass.Password;
                ProxySettingsProp[2] = Domain.Text;
            }
        }

        private void proxy_Checked(object sender, RoutedEventArgs e)
        {
            RequireProxy = (bool)Proxy.IsChecked;
            if (RequireProxy)
            {
                // force username and domain to not be null
                proxy_TextChanged(null, null);
                ProxyGrid.Visibility = Visibility.Visible;
            }
            else
            {
                ProxyGrid.Visibility = Visibility.Collapsed;
                VerifyFields();
            }
        }
    }
}
