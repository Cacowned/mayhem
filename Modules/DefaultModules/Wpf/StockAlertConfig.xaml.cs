using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using MayhemWpf.UserControls;
using System.Net;

namespace DefaultModules.Wpf
{
    /// <summary>
    /// This is the configuration dialog responsible for error-checking
    /// and user-specified information-gaterhing used in conjunction with StockAlert.cs
    /// </summary>
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

        /// <summary>
        /// Creates the configuration dialog for StockAlert
        /// </summary>
        /// <param name="stockSymbol">stock symbol</param>
        /// <param name="stockPrice">stock price</param>
        /// <param name="changeParam">true if watching for change in price, false if watching stock price (no delta)</param>
        /// <param name="abovePrice">whether to watch above or below target price / change</param>
        /// <param name="alwaysTrigger">trigger once or every time</param>
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
            StockPrice.Text = StockPriceProp.ToString(CultureInfo.InvariantCulture);
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

        /// <summary>
        /// Check that the stock price length is over 0 (not blank) and that it is a number
        /// positive if checking last trade, pos or neg if checking change in price
        /// </summary>
        private void VerifyFields()
        {
            string error = "Invalid";
            if (!timer.IsEnabled)
            {
                double price = 0.0;
                bool isNumber = StockPrice.Text.Length > 0 && double.TryParse(StockPrice.Text, out price);

                // if checking trades, want positive numbers only, otherwise true
                bool posPrice = (bool)LastTrade.IsChecked ? (price > 0) : true;

				bool badPrice = !(isNumber && posPrice);
				bool badSymbol = !(StockSymbol.Text.Length > 0 && IsValidStock());

				if (badPrice && badSymbol)
				{
					error += " price and stock symbol";
				}
				else if (badPrice)
				{
					error += " price";
				}
				else if (badSymbol)
				{
					error += " stock symbol";
				}

                CanSave = error.Equals("Invalid");
            }
            else
            {
                error = "Cannot connect to the Internet";
            }

            TextChanged(error);
        }

        /// <summary>
        /// The api returns xml but no company name if the stock does not exist,
        /// returns true if the stock exitst
        /// </summary>
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

        /// <summary>
        /// Parses xml for current price and company name
        /// </summary>
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

        /// <summary>
        /// If the stock name is changed, verify fields and update 
        /// </summary>
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

        /// <summary>
        /// When the asking price is changed
        /// </summary>
        private void UpdateAsking(object sender, RoutedEventArgs e)
        {
            ChangeProp = !(bool)LastTrade.IsChecked;
            VerifyFields();
        }

        /// <summary>
        /// Checks for an internet connection
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
    }
}