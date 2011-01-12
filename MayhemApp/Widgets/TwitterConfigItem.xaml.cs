using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MayhemApp.Business_Logic.Twitter;
using System.Diagnostics;
using MayhemApp.Dialogs;

namespace MayhemApp
{
    /// <summary>
    /// Interaction logic for TwitterConfigItem.xaml
    /// </summary>
    public partial class TwitterConfigItem : ListBoxItem
    {
        public static string TAG = "[TwitterConfigItem] : ";
      

        public TwitterConfigItem()
        {
            InitializeComponent();

            itemOnOffControl.OnSliderOn += new OnOffControl.SliderMovedEventHandler(itemOnOffControl_OnSliderOn);
            itemOnOffControl.OnSliderOff += new OnOffControl.SliderMovedEventHandler(itemOnOffControl_OnSliderOff);

            MayhemTwitter twitter = MayhemTwitter.Instance;

            if (twitter.twitterState == MayhemTwitter.TWITTER_STATE.ON)
            {
                // slide the button over
                itemOnOffControl.Set_On(false);
            }
            
        }

        void itemOnOffControl_OnSliderOff(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            MayhemTwitter t = MayhemTwitter.Instance;

            if (t.twitterState == MayhemTwitter.TWITTER_STATE.ON)
            {
                // turn of twitter
                t.twitterState = MayhemTwitter.TWITTER_STATE.OFF_TOKEN;
                t.SaveTwitterSettings();
            }
        }

        void itemOnOffControl_OnSliderOn(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            MayhemTwitter t = MayhemTwitter.Instance;

            // TODO: Test the reset checkbox
            if (t.twitterState == MayhemTwitter.TWITTER_STATE.OFF_NOTOKEN ||
                (t.twitterState == MayhemTwitter.TWITTER_STATE.OFF_TOKEN && (bool) resetCheckBox.IsChecked) ) 
            {
                // get request Token

                if ((bool) resetCheckBox.IsChecked)
                {
                    resetCheckBox.IsChecked = false;
                }

                if (t.GetRequestToken())
                {
                    Debug.WriteLine(TAG + "Got Request Token...");
                    Uri requestURI = t.BuildAuthURI();

                    // now open a browser window!!
                    BrowserWindow b = new BrowserWindow();
                    // if successful, the browser updates the twitterstate to ON
                    b.Show();
                    b.DisplayURI(requestURI);
                    b.browser.Navigated += new NavigatedEventHandler(browser_Navigated);
                   

                }

            }
            else if (t.twitterState == MayhemTwitter.TWITTER_STATE.OFF_TOKEN)
            {
                t.twitterState = MayhemTwitter.TWITTER_STATE.ON;
                t.SaveTwitterSettings();
            }

        }

        void browser_Navigated(object sender, NavigationEventArgs e)
        {
            //throw new NotImplementedException();

            WebBrowser b = sender as WebBrowser;

           // b.n
        }

       
    }
}
