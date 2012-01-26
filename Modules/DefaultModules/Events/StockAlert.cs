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
using System.Net;

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
        private string queryParam;

        [DataMember]
        private bool abovePrice;

        private bool hasPassed;
        private DispatcherTimer timer;
        private bool internetFlag;

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = (StockAlertConfig)configurationControl;
            stockSymbol = config.StockSymbolProp;
            stockPrice  = config.StockPriceProp;
            queryParam  = config.QueryParamProp;
            abovePrice  = config.WatchAboveProp;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new StockAlertConfig(stockSymbol, stockPrice, queryParam, abovePrice); }
        }

        public string GetConfigString()
        {
            string above_below = abovePrice ? "above " : "below ";
            //above_below += stockPrice;
            return String.Format("Watching {0} for {1} ${2}", stockSymbol, above_below, stockPrice);
            //return String.Format("Watching {0} for {1}F", stockSymbol, above_below);
        }

        protected override void OnLoadDefaults()
        {
            stockSymbol = "MSFT";
            stockPrice = 32.05;
            queryParam = "k1";
            abovePrice = false;
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
            if (!ConnectedToInternet())
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.Internet_NotConnected);
            }
            timer.Start();
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            timer.Stop();
        }
        #endregion

        private void CheckStock(object sender, EventArgs e)
        {
            // Test for internet connection
            if (ConnectedToInternet())
            {
                internetFlag = true;
                // Retrieve XML document  
                WebClient wc = new WebClient();
                string file = wc.DownloadString("http://finance.yahoo.com/d/quotes.csv?s=RHT+MSFT&f=sb2b3jk");
                string[] parts = file.Split(new Char[] { '\n' });


                // logic for when to Trigger()
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
