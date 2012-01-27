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
        private XmlTextReader stockData;

        public override string Title
        {
            get
            {
                return "RSS Feed Alert";
            }
        }

        public FeedAlertConfig()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 2);
            //timer.Tick += CheckInternet;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            //StockSymbol.Text = StockSymbolProp;
        }

        public override void OnSave()
        {
           // StockSymbolProp = StockSymbol.Text;
        }

        private void CheckInternet(object sender, EventArgs e)
        {
            // don't want to add overloads to ConnectedToInternet()
            ConnectedToInternet();
        }

        private void VerifyFields()
        {
            string error = "Invalid";
            IsValidStock();
            CanSave = error.Equals("Invalid");
            TextChanged(error);
        }

        private bool IsValidStock()
        {
            if (ConnectedToInternet())// && StockSymbol.Text.Length > 0)
            {
                stockData = new XmlTextReader("http://www.npr.org/rss/rss.php?id=1012");
                XmlDocument rssDoc = new XmlDocument();
                rssDoc.Load(stockData);

                MessageBox.Show(rssDoc.ChildNodes.Count.ToString());

                for (int i = 0; i < rssDoc.ChildNodes.Count; i++)
                {
                    if (rssDoc.ChildNodes[i].Name == "rss")
                    {
                        MessageBox.Show(rssDoc.ChildNodes[i].InnerText);
                    }
                    
                }
                /*
                // check that there is xml data
                stockData.ReadToFollowing("company");
                if (stockData.GetAttribute("data").Equals(""))
                {
                    StockName.Text = "Stock";
                    return false;
                }
                 */
                return true;
            }
            return false;
        }

        private void TextChanged(string text)
        {
            textInvalid.Text = text;
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            if (textInvalid.Visibility == Visibility.Visible)
            {
                FeedName.Text = "Feed";
            }
            if (CanSave && !timer.IsEnabled)
            {
                //CheckStock();
            }
        }

        private void CheckStock()
        {
            //stockData.ReadToFollowing("company");
            string companyName = stockData.GetAttribute("data");

            // get current stock price
            stockData.ReadToFollowing("last");
            string currentPrice = stockData.GetAttribute("data");

            FeedName.Text = String.Format("{0} - ${1}", companyName, currentPrice);
        }

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int desciption, int reservedValue);

        private bool ConnectedToInternet()
        {
            int desc;
            try
            {
                if (InternetGetConnectedState(out desc, 0))
                {
                    if (timer.IsEnabled)
                    {
                        timer.Stop();
                        VerifyFields();
                    }
                    return true;
                }
            }
            catch { }

            if (!timer.IsEnabled)
            {
                timer.Start();
            }
            return false;
        }

        private void Stock_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!timer.IsEnabled)
            {
                VerifyFields();
            }
            else
            {
                FeedName.Text = "Stock";
                TextChanged("Cannot connect to the Internet");
            }
        }

        private void URL_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
