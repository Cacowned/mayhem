using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Threading;
using System.Xml;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DefaultModules.Events
{
    [DataContract]
    [MayhemModule("Stock Alert", "Monitors changes in stock prices")]

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

        private bool hasPassed;
        private DispatcherTimer timer;
        private bool internetFlag;

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = (StockAlertConfig)configurationControl;
            stockSymbol = config.StockSymbolProp;
            stockPrice = config.StockPriceProp;
            changeParam = config.ChangeProp;
            abovePrice = config.WatchAboveProp;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new StockAlertConfig(stockSymbol, stockPrice, changeParam, abovePrice); }
        }

        public string GetConfigString()
        {
            string above_below = abovePrice ? "above " : "below ";
            string change = changeParam ? "change " : "last trade ";
            return String.Format("{0} for {1} {2} ${3}", stockSymbol, change, above_below, stockPrice);
        }

        protected override void OnLoadDefaults()
        {
            stockSymbol = "MSFT";
            stockPrice = 32.05;
            changeParam = false;
            abovePrice = false;
        }

        protected override void OnAfterLoad()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 10);
            timer.Tick += CheckStock;
            hasPassed = false;
        }

        #region Timer
        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!ConnectedToInternet())
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.Internet_NotConnected);
            }
            timer.Start();
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            timer.Stop();
            hasPassed = false;
        }
        #endregion

        private void CheckStock(object sender, EventArgs e)
        {
            // Test for internet connection
            if (ConnectedToInternet())
            {
                internetFlag = true;
                // Retrieve XML document  
                XmlTextReader stockData = new XmlTextReader("http://www.google.com/ig/api?stock=" + stockSymbol);

                string readTo = changeParam ? "change" : "last";
                stockData.ReadToFollowing(readTo);
                double livePrice = Double.Parse(stockData.GetAttribute("data"));

                // if the stock is above the watching price and user wants to trigger above
                // OR
                // if stock is below watching price and the user wants to trigger below
                // stock price > 0 for all non-negative testing
                if (stockPrice > 0 && ((abovePrice && livePrice >= stockPrice) || (!abovePrice && livePrice <= stockPrice)) ||
                   ((abovePrice && livePrice <= stockPrice) || (!abovePrice && livePrice >= stockPrice)))
                {
                    // logic for when to Trigger()
                    // trigger once when passed, that's it
                    if (!hasPassed)
                    {
                        hasPassed = true;
                        Trigger();
                    }
                }
            }
            else if (internetFlag)
            {
                internetFlag = false;
                ErrorLog.AddError(ErrorType.Warning, Strings.Internet_NotConnected);
            }
        }

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int desciption, int reservedValue);

        private static bool ConnectedToInternet()
        {
            int desc;
            try
            {
                return InternetGetConnectedState(out desc, 0);
            }
            catch
            {
                return false;
            }
        }
    }
}
