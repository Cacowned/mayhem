using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using MayhemApp.Business_Logic.Twitter;

namespace MayhemApp.Dialogs
{
    /// <summary>
    /// Interaction logic for BrowserWindow.xaml
    /// </summary>
    public partial class BrowserWindow : Window
    {
        public static string TAG = "[BrowserWindow] : ";

        public class SaveButtonClickEventArgs : RoutedEventArgs
        {
            public string textBoxValue;

            public SaveButtonClickEventArgs(string fValue)
            {
                textBoxValue = fValue;
            }
        }

        public delegate void SaveButtonClickHandler(object sender, SaveButtonClickEventArgs e);
      
        public BrowserWindow()
        {
            InitializeComponent();

            browser.Navigated += new System.Windows.Navigation.NavigatedEventHandler(browser_Navigated);
        }

        void browser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Debug.WriteLine(TAG + "browser_Navigated");
            browser.BringIntoView();
        }

        public void WindowCloseButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public void DisplayURI(Uri uri)
        {
            Debug.WriteLine(TAG + " navigating to URI " + uri);
            browser.Navigate(uri);
         
        }

        private static Regex _isNumber = new Regex(@"^\d+$");

        public static bool IsInteger(string theValue)
        {
            Match m = _isNumber.Match(theValue);
            return m.Success;
        }

       

        private void SaveButton_Clicked(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(TAG + "Save Button Clicked");
            
            // do the twitter stuff directly here
            MayhemTwitter twitter = MayhemTwitter.Instance;

            string userEntry = twitter_code_field.Text;

            if (userEntry.Length > 0 && IsInteger(userEntry))
            {
                // field entry seems to be OK

                if (twitter.GetAccessToken(userEntry))
                {
                   twitter.twitterState = MayhemTwitter.TWITTER_STATE.ON;
                   twitter.SaveTwitterSettings();
                   MessageBoxResult r =   MessageBox.Show("Twitter Connection Successful");
                   if (r == MessageBoxResult.OK)
                   {
                       this.Close();
                   }
                }
                else
                {
                    MessageBox.Show("Twitter could not authorize Mayhem's access. Did you enter the code correctly?");
                }
            }
            else
            {
                MessageBox.Show("The code shown by Twitter needs to be entered or pasted into the field exactly as shown.");
            }
        }
    }
}
