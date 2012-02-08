// This is the configuration dialog responsible for error-checking
// and user-specified information-gaterhing used in conjunction with StockAlert.cs

using System;
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

        public bool TriggerEveryProp
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

        // Takes: 
        // string -> stock symbol
        // double -> stock price
        // bool   -> true if watching for change in price, false if watching stock price (no delta)
        // bool   -> wether to watch above or below target price / change
        // bool   -> trigger once or every time
        public StockAlertConfig(string stockSymbol, double stockPrice, bool changeParam, bool abovePrice, bool alwaysTrigger)
        {
            StockSymbolProp = stockSymbol;
            StockPriceProp = stockPrice;
            ChangeProp = changeParam;
            WatchAboveProp = abovePrice;
            TriggerEveryProp = alwaysTrigger;

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

            TriggerEvery_Check.IsChecked = TriggerEveryProp;
        }

        public override void OnSave()
        {
            StockSymbolProp = StockSymbol.Text;
            StockPriceProp = Math.Round(Convert.ToDouble(StockPrice.Text), 2);
            WatchAboveProp = (bool)Above.IsChecked;
            TriggerEveryProp = (bool)TriggerEvery_Check.IsChecked;
        }

        // Didn't want to add overloads to ConnectedToInternet()
        private void CheckInternet(object sender, EventArgs e)
        {
            ConnectedToInternet();
        }

        // Check that the stock price length is over 0 (not blank) and that it is a number
        // positive if checking last trade, pos or neg if checking change in price
        private void VerifyFields()
        {
            string error = "Invalid";
            if (!timer.IsEnabled)
            {
                double price = 0.0;
                bool isNumber = StockPrice.Text.Length > 0 && double.TryParse(StockPrice.Text, out price);

                // if checking trades, want positive numbers only, otherwise true
                bool posPrice = (bool)LastTrade.IsChecked ? (price > 0) : true;

                error += isNumber && posPrice ? string.Empty : " price";
                error += StockSymbol.Text.Length > 0 && IsValidStock() ? string.Empty : " stock symbol";

                CanSave = error.Equals("Invalid");
            }
            else
            {
                error = "Cannot connect to the Internet";
            }

            TextChanged(error);
        }

        // The api returns xml but no company name if the stock does not exist,
        // returns true if the stock exitst
        private bool IsValidStock()
        {
            try
            {
                stockData = new XmlTextReader("http://www.google.com/ig/api?stock=" + StockSymbol.Text);

                // check that there is xml data
                stockData.ReadToFollowing("company");
                if (stockData.GetAttribute("data").Equals(string.Empty))
                {
                    StockName.Text = "Stock";
                    return false;
                }

                return true;
            }
            catch
            {
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
            else
            {
                StockName.Text = "Stock";
            }
        }

        // Parses xml for current price and company name
        private void CheckStock()
        {
            string companyName = stockData.GetAttribute("data");

            // get current stock price
            stockData.ReadToFollowing("last");
            string currentPrice = stockData.GetAttribute("data");

            StockName.Text = companyName;
            CurrentStockPrice.Text = "$" + currentPrice;
            stockData.Close();
        }

        // If the stock name is changed, verify fields and update 
        private void Stock_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (ConnectedToInternet())
            {
                VerifyFields();
            }
            else
            {
                StockName.Text = "Stock Name";
                CurrentStockPrice.Text = "Stock Price";
                TextChanged("Cannot connect to the Internet");
            }
        }

        // When the asking price is changed
        private void UpdateAsking(object sender, RoutedEventArgs e)
        {
            ChangeProp = !(bool)LastTrade.IsChecked;
            VerifyFields();
        }

        // Checks for an internet connection
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