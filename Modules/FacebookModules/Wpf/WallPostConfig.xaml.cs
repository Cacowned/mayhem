using MayhemWpf.UserControls;
using Facebook;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Runtime.InteropServices;
using FacebookModules.LowLevel;

namespace FacebookModules.Wpf
{

    public partial class WallPostConfig : WpfConfiguration
    {
        public string TokenProp
        {
            get;
            private set;
        }

        public string UserNameProp
        {
            get;
            private set;
        }

        private const string appId = "156845751071300";
        private string[] extendedPermissions = new[] { "user_about_me", "offline_access" };
        private Facebook.FacebookOAuthResult result;

        public WallPostConfig()
        {
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
            TokenProp = "AAACOpn9G8kQBAGwVp51kgtSJx9DaZA9A3afryspA4Gpsz2EFt8EQWd2vx0aSpvehH6jZC8TeuHNMFNZBia61MeM7VDKSuoZD";
            // otherwise make this null or blank, to check in CS file

            LoginAttempt();

            //webBrowser.Visibility = Visibility;
            // connect to facebook, pop up dialog, etc... store token,

            
            if(FacebookOAuthResult != null || true)
                LoadUser();
            CanSave = FacebookOAuthResult != null ? true : true ;
        }

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

        private void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
           // FacebookOAuthResult result;

           // MessageBox.Show(e.ExtraData.ToString());
            /*
            if (FacebookOAuthResult.TryParse(e.Url, out result))
            {
                this.FacebookOAuthResult = result;
                //this.DialogResult = result.IsSuccess ? DialogResult.OK : DialogResult.No;
                TokenProp = result.AccessToken;
            }
            else
            {
                this.FacebookOAuthResult = null;
            }
             * */
            
            var fb = new FacebookClient(TokenProp);
            dynamic result = fb.Get("/me");
            UserNameProp = result.name;
        }

        public FacebookOAuthResult FacebookOAuthResult { get; private set; }
    }
}
