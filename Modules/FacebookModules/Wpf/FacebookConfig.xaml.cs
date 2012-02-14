using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Facebook;
using FacebookModules.LowLevel;
using MayhemWpf.UserControls;

namespace FacebookModules.Wpf
{

    public partial class FacebookConfig : WpfConfiguration
    {
        public string TokenProp
        {
            get;
            private set;
        }

        private const string appId = "156845751071300";
        private string[] extendedPermissions = new[] { "offline_access" };

        public FacebookConfig(string token)
        {
            TokenProp = token;
            InitializeComponent();
        }

        public override string Title
        {
            get
            {
                return "FaceBook";
            }
        }

        public override void OnLoad()
        {
            CanSave = false;
            if (TokenProp == null)
            {
                browserContainer.Visibility = Visibility.Visible;
                LoginAttempt();
            }
            else
            {
                // TODO
                // test the current token to make sure it's still valid
            }
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

                Uri loginUrl = oauth.GetLoginUrl(parameters);
                webBrowser.Navigate(loginUrl);
            }
            else
            {
                textInvalid.Text = "Cannot connect to Internet";
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
        }

        /// <summary>
        /// Tries to parse the login token from the web browser response
        /// If so, loads the user and enables saving
        /// </summary>
        private void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            browserContainer.Visibility = Visibility.Collapsed;
            FacebookOAuthResult result = null;
            if (FacebookOAuthResult.TryParse(e.Url, out result))
            {
                TokenProp = result.AccessToken;
            }

            if (result != null)
                LoadUser();
            CanSave = result != null;
        }
    }
}
