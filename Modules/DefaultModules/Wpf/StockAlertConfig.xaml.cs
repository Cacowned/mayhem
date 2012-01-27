using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using MayhemWpf.UserControls;

namespace DefaultModules.Wpf
{
    public partial class StockAlertConfig : WpfConfiguration
    {
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

        public bool ChangeProp
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

        private DispatcherTimer timer;
        private XmlReader stockData;

        public StockAlertConfig(string stockSymbol, double stockPrice, bool changeParam, bool abovePrice)
        {
            StockSymbolProp = stockSymbol;
            StockPriceProp = stockPrice;
            ChangeProp = changeParam;
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
            if (!ChangeProp)
            {
                LastTrade.IsChecked = true;
            }
            else
            {
                Change.IsChecked = true;
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

            double price = 0.0;
            bool isNumber = StockPrice.Text.Length > 0 && double.TryParse(StockPrice.Text, out price);

            // if checking trades, want positive numbers only, otherwise true
            bool posPrice = (bool)LastTrade.IsChecked ? (price > 0) : true;

            error += isNumber && posPrice ? "" : " price";
            error += IsValidStock() ? "" : " stock symbol";

            CanSave = error.Equals("Invalid");
            TextChanged(error);
        }

        private bool IsValidStock()
        {
            if (ConnectedToInternet() && StockSymbol.Text.Length > 0)
            {
                try
                {
                    stockData = new XmlTextReader("http://www.google.com/ig/api?stock=" + StockSymbol.Text);

                    // check that there is xml data
                    stockData.ReadToFollowing("company");
                    if (stockData.GetAttribute("data").Equals(""))
                    {
                        StockName.Text = "Stock";
                        return false;
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }

        private void TextChanged(string text)
        {
            textInvalid.Text = text;
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            if (textInvalid.Visibility == Visibility.Visible)
            {
                StockName.Text = "Stock";
            }
            if (CanSave && !timer.IsEnabled)
            {
                CheckStock();
            }
        }

        private void CheckStock()
        {
            string companyName = stockData.GetAttribute("data");

            // get current stock price
            stockData.ReadToFollowing("last");
            string currentPrice = stockData.GetAttribute("data");

            StockName.Text = String.Format("{0} - ${1}", companyName, currentPrice);
            stockData.Close();
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

        private void UpdateAsking(object sender, RoutedEventArgs e)
        {
            ChangeProp = !(bool)LastTrade.IsChecked;
            VerifyFields();
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
    }
}
