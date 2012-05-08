using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Facebook;
using FacebookModules.LowLevel;
using FacebookModules.Wpf.UserControls;
using MayhemWpf.UserControls;
using System.Threading;

namespace FacebookModules.Wpf
{

    public partial class FacebookConfigDebug : WpfConfiguration
    {
        public string TokenProp
        {
            get;
            private set;
        }

        private const string appId = "249936281752098";
        private string[] extendedPermissions = new[] { "publish_stream", "read_mailbox", "read_stream", "manage_notifications", "offline_access" };
        private string title;
        
        // for debug
        Uri loginUrl;
        public FacebookConfigControl ControlItem { get; private set; }
   

        public FacebookConfigDebug(string token, string title, FacebookConfigControl control)
        {
            TokenProp = token;
            this.title = title;
            if (control != null)
                this.ControlItem = control;
            InitializeComponent();
        }

        public override void OnSave()
        {
            if (ControlItem != null)
                ControlItem.OnSave();
        }

        public override string Title
        {
            get
            {
                return "Facebook: " + title;
            }
        }

        public override void OnLoad()
        {
            CanSave = false;
            if (ControlItem != null)
            {
                facebookControl.Content = ControlItem;
                ControlItem.OnRevalidate += Revalidate;
                ControlItem.OnLoad();
            }

            LoginAttempt();
        }

        /// <summary>
        /// Tries to login to facebook, fires browser_navigated after completed or 
        /// if there is no Internet, displays and error
        /// </summary>
        private void LoginAttempt()
        {
            if (Utilities.ConnectedToInternet())
            {
                var oauth = new FacebookOAuthClient { AppId = appId };

                var parameters = new Dictionary<string, object>
                    {
                        { "response_type", "token" },
                        { "display", "popup" }
                    };

                if (extendedPermissions != null && extendedPermissions.Length > 0)
                {
                    var scope = new StringBuilder();
                    scope.Append(string.Join(",", extendedPermissions));
                    parameters["scope"] = scope.ToString();
                }

                loginUrl = oauth.GetLoginUrl(parameters);
                webBrowser.Navigate(loginUrl);
            }
            else
            {
                CanSave = false;
                textInvalid.Text = "Cannot connect to Internet";
                textInvalid.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Gets the profile picture and name for the current user and displays it
        /// in the dialog
        /// </summary>
        private void LoadUser()
        {
            var fb = new FacebookClient(TokenProp);

            dynamic result = fb.Get("/me");
            var uId = result.id;
            User_Info.Text = result.name;

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(String.Format("http://graph.facebook.com/{0}/picture", uId), UriKind.Absolute);
            bi.EndInit();

            Profile.Source = bi;

            // for login success
            CanSave = true;
            Revalidate();
            // remove after debug
            webBrowser.Navigate("www.facebook.com");
        }

        /// <summary>
        /// Tries to parse the login token from the web browser response
        /// If so, loads the user and enables saving
        /// </summary>
        private void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {

            FacebookOAuthResult result = null;
            if (FacebookOAuthResult.TryParse(e.Url, out result))
            {
                TokenProp = result.AccessToken;
            }

            if (result != null)
                LoadUser();
        }

        private void Revalidate()
        {
            // get sensor stuff
            if (TokenProp != null && ControlItem != null)
            {
                CanSave = ControlItem.CanSave;
            }
        }
    }
}
