using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using MayhemWpf.UserControls;
using System.Xml;

namespace DefaultModules.Wpf
{
    public partial class StockAlertConfig : WpfConfiguration
    {

        private DispatcherTimer timer;
        private XmlTextReader stockData;

        public string StockSymbolProp
        {
            get;
            private set;
        }

        public double StockPriceProp
        {
            get;
            private set;
        }

        public string QueryParamProp
        {
            get;
            private set;
        }

        public bool WatchAboveProp
        {
            get;
            private set;
        }

        public override string Title
        {
            get
            {
                return "Stock Alert";
            }
        }

        public StockAlertConfig(string stockSymbol, double stockPrice, string queryParam, bool abovePrice)
        {
            StockSymbolProp = stockSymbol;
            StockPriceProp = stockPrice;
            QueryParamProp = queryParam;
            WatchAboveProp = abovePrice;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.Tick += CheckInternet;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            StockSymbol.Text = StockSymbolProp;
            StockPrice.Text = StockPriceProp.ToString();
            if (QueryParamProp.Equals("k1"))
            {
                LastTrade.IsChecked = true;
            }
            else
            {
                Asking.IsChecked = true;
            }

            if (WatchAboveProp)
            {
                Above.IsChecked = true;
            }
            else
            {
                Below.IsChecked = true;
            }
        }

        public override void OnSave()
        {
            StockSymbolProp = StockSymbol.Text;
            StockPriceProp = Math.Round(Convert.ToDouble(StockPrice.Text), 2);
            // explicitly test, check if saves propper value
            // QueryParamProp = (bool)LastTrade.IsChecked ? "k1" : "b2";
            WatchAboveProp = (bool)Above.IsChecked;
        }

        private void CheckInternet(object sender, EventArgs e)
        {
            // don't want to add overloads to ConnectedToInternet()
            ConnectedToInternet();
        }

        private void VerifyFields()
        {
            string error = "Invalid";

            double price;
            bool isAboveZero = StockPrice.Text.Length > 0 && double.TryParse(StockPrice.Text, out price) && (price > 0);

            error += isAboveZero ? "" : " price";
            error += IsValidStock() ? "" : " stock symbol";

            CanSave = error.Equals("Invalid");
            TextChanged(error);
        }

        private bool IsValidStock()
        {
            if (ConnectedToInternet() && StockSymbol.Text.Length > 0)
            {
                /* switched to google api because it's MUCH faster
                WebClient wc = new WebClient();
                string query = String.Format("http://finance.yahoo.com/d/quotes.csv?s={0}&f=s{1}", StockSymbol.Text, QueryParamProp);
                //query = "http://finance.yahoo.com/d/quotes.csv?s=RHT+MSFT&f=sb2b3jk";
                 */
                stockData = new XmlTextReader(String.Format("http://www.google.com/ig/api?stock=" + StockSymbol.Text));
                //MessageBox.Show(query);
                //string file = wc.DownloadString(query);
                //string[] parts = file.Split(new Char[] { '\n' });
                //MessageBox.Show(file.Trim());

                StockName.Text = "";

                return true;
            }
            return false;
        }

        private void TextChanged(string text)
        {
            textInvalid.Text = text;
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            if (CanSave && !timer.IsEnabled)
            {
                CheckStock();
            }
        }

        private void CheckStock()
        {

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
                StockSymbol.Text = "Stock";
                TextChanged("Cannot connect to the Internet");
            }
        }

        private void UpdateQuery(object sender, RoutedEventArgs e)
        {
            QueryParamProp = (bool)LastTrade.IsChecked ? "k1" : "b2";
        }
    }
}
