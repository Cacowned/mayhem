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
    [MayhemModule("Stock Alert", "Monitors changes in stock prices")]

    /// <summary>
    /// This module allows a user to enter a stock symbol
    /// trigger price, and allows them to choose if they 
    /// want an event to trigger when that price is passed while going down
    /// or passed while going up. The user can also specify a change in price (delta)
    /// to watch and wether to trigger when the delta is more or less than they specify.
    /// <summary>
    public class StockAlert : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string stockSymbol;

        [DataMember]
        private double stockPrice;

        [DataMember]
        private bool changeParam;

        [DataMember]
        private bool abovePrice;

        [DataMember]
        private bool triggerEvery;

        private bool hasPassed;
        private DispatcherTimer timer;
        private bool internetFlag;

        // the amount of "buffer" that the price has to go passed to re-enable trigger
    	private const double PriceOffset = 1.0;

    	public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = (StockAlertConfig)configurationControl;
            stockSymbol = config.StockSymbolProp;
            stockPrice = config.StockPriceProp;
            changeParam = config.ChangeProp;
            abovePrice = config.WatchAboveProp;
            triggerEvery = config.TriggerEveryProp;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new StockAlertConfig(stockSymbol, stockPrice, changeParam, abovePrice, triggerEvery); }
        }

        /// <summary>
        /// {Stock symbol} for {change or last trade} {above or below} ${price}
        /// </summary>
        public string GetConfigString()
        {
            string aboveBelow = abovePrice ? "above " : "below ";
            string change = changeParam ? "change " : "last trade ";
            return String.Format("{0} for {1} {2} ${3}", stockSymbol, change, aboveBelow, stockPrice);
        }

        /// <summary>
        /// Default stock Microsoft, price of 32.05
        /// watching below, not watching delta and trigger on
        /// every pass is true
        /// </summary>
        protected override void OnLoadDefaults()
        {
            stockSymbol = "MSFT";
            stockPrice = 32.05;
            changeParam = false;
            abovePrice = false;
            triggerEvery = true;
        }

        protected override void OnAfterLoad()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 1, 0);
            timer.Tick += CheckStock;
            hasPassed = false;
        }

        #region Timer
        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!Utilities.ConnectedToInternet())
            {
                ErrorLog.AddError(ErrorType.Warning, String.Format(Strings.Internet_NotConnected, "Stock"));
            }

            timer.Start();
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            timer.Stop();
            hasPassed = false;
        }
        #endregion

        /// <summary>
        /// Retrieve the stock price or delta (change in price)
        /// depending on what the user specified to watch for
        /// </summary>
        private void CheckStock(object sender, EventArgs e)
        {
            // Test for internet connection
            if (Utilities.ConnectedToInternet())
            {
                internetFlag = true;
                try
                {
                    // Retrieve XML document 
                    string url = "http://www.google.com/ig/api?stock=" + stockSymbol;

                    WebRequest webRequest = WebRequest.Create(url);
                    HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                    webRequest.CachePolicy = noCachePolicy;

                    using (WebResponse webResponse = webRequest.GetResponse())
                    {
                        using (Stream responseStream = webResponse.GetResponseStream())
                        {
                            StreamReader s = new StreamReader(responseStream, Encoding.GetEncoding(1252));
                            XmlReader stockData = XmlReader.Create(s);

                            string readTo = changeParam ? "change" : "last";
                            stockData.ReadToFollowing(readTo);
                            double livePrice = Double.Parse(stockData.GetAttribute("data"));

                            // if the stock is above the watching price and user wants to trigger above
                            // OR
                            // if stock is below watching price and the user wants to trigger below
                            // stock price > 0 for all non-negative testing
                            if ((stockPrice > 0 && ((abovePrice && livePrice >= stockPrice) || (!abovePrice && livePrice <= stockPrice))) ||
                               ((abovePrice && livePrice <= stockPrice) || (!abovePrice && livePrice >= stockPrice)))
                            {
                                // logic for when to Trigger()
                                // trigger once when passed, then trigger again once barrier has been reset
                                // ELSE IF
                                // trigger on every pass AND the stock price is at a "reset point" enables ability to trigger on re-pass of point
                                if (!hasPassed)
                                {
                                    hasPassed = true;
                                    Trigger();
                                }
                                else if ((triggerEvery && (abovePrice && livePrice >= stockPrice + PriceOffset)) || (!abovePrice && livePrice <= stockPrice - PriceOffset))
                                {
                                    hasPassed = false;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            else if (internetFlag)
            {
                internetFlag = false;
                ErrorLog.AddError(ErrorType.Warning, String.Format(Strings.Internet_NotConnected, "Stock"));
            }
        }
    }
}
