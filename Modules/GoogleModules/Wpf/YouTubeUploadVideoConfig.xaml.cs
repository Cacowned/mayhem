using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using GoogleModules.Resources;
using MayhemCore;
using MayhemWpf.UserControls;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// User Control for setting the information about the video an user wants to upload.
    /// </summary>
    public partial class YouTubeUploadVideoConfig : WpfConfiguration
    {
        public string VideoTitle
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public string Category
        {
            get;
            private set;
        }

        public string VideoPath
        {
            get;
            private set;
        }

        public string RefreshToken
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;
        private const string Scope = "https://gdata.youtube.com";
        private string authorizationCode;
        private OAuth2Authenticator<NativeApplicationClient> auth;

        private static AutoResetEvent eventAuthorizationCodeEnter = new AutoResetEvent(false);
        private static AutoResetEvent eventWaitAuthorization = new AutoResetEvent(false);

        private Thread loadTokenThread;

        private bool authenticationFailed;
        private bool isAuthenticated;
        private bool canEnableCheckCode;

        public YouTubeUploadVideoConfig(string videoTitle, string description, string category, string videoPath, string title)
        {
            InitializeComponent();

            List<string> categories = new List<string>()
            {
                "Autos & Vehicles", "Comedy", "Education", "Entertainment", "Film & Animation", "Gaming", "Howto & Style", "Music", "News & Politics", 
                "Nonprofits & Activism", "People & Blogs", "Pets & Animals", "Science & Technology", "Sports", "Travel & Events"
            };

            CategoryComboBox.Items.Clear();
            foreach (string cat in categories)
            {
                CategoryComboBox.Items.Add(cat);
            }

            VideoTitle = videoTitle;
            Description = description;
            Category = category;
            VideoPath = videoPath;
            configTitle = title;
        }

        public override void OnLoad()
        {
            CanSave = true;
            buttonCheckCode.IsEnabled = false;
            canEnableCheckCode = false;

            VideoTitleBox.Text = VideoTitle;
            DescriptionBox.Text = Description;
            VideoPathBox.Text = VideoPath;

            if (!string.IsNullOrEmpty(Category))
            {
                CategoryComboBox.SelectedItem = Category;
            }
            else
            {
                CategoryComboBox.SelectedIndex = 0;
            }

            CheckValidity();
        }

        public override void OnSave()
        {
            VideoTitle = VideoTitleBox.Text;
            Description = DescriptionBox.Text;
            Category = CategoryComboBox.SelectedItem as string;
            VideoPath = VideoPathBox.Text;
        }

        private void buttonAuthenticate_Click(object sender, RoutedEventArgs e)
        {
            NativeApplicationClient provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = Strings.Google_ClientID;
            provider.ClientSecret = Strings.Google_ClientSecret;

            auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);

            loadTokenThread = new Thread(new ThreadStart(auth.LoadAccessToken));
            loadTokenThread.Start();
        }

        protected IAuthorizationState GetAuthorization(NativeApplicationClient arg)
        {
            authenticationFailed = false;
            isAuthenticated = false;

            IAuthorizationState state = null;

            canEnableCheckCode = true;

            try
            {
                // Get the auth URL.
                state = new AuthorizationState(new[] { Scope });
                state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
                Uri authUri = arg.RequestUserAuthorization(state);

                // Request authorization from the user (by opening a browser window).
                Process.Start(authUri.ToString());

                eventAuthorizationCodeEnter.WaitOne();

                // Retrieve the access token by using the authorization code.
                return arg.ProcessUserAuthorization(authorizationCode, state);
            }
            catch (Exception ex)
            {
                authenticationFailed = true;

                Logger.Write(ex);
                return null;
            }
            finally
            {
                if (authenticationFailed)
                {
                    isAuthenticated = false;
                }
                else
                {
                    isAuthenticated = true;

                    if (state != null)
                    {
                        RefreshToken = state.RefreshToken;
                    }
                }

                // The user needs to authenticate again in order to get another code.
                canEnableCheckCode = false;
                eventWaitAuthorization.Set();
            }
        }

        private void buttonCheckCode_Click(object sender, RoutedEventArgs e)
        {
            authorizationCode = AuthorizationCodeBox.Text;

            eventAuthorizationCodeEnter.Set();

            // We need to wait for the authentication to take place. If in 15 seconds the authentication doesn't take place we stop it. 
            Thread.Sleep(500);
            eventWaitAuthorization.WaitOne(20000);

            CheckValidity();
            buttonCheckCode.IsEnabled = false;
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            if (dlg.ShowDialog().Equals(DialogResult.OK))
            {
                VideoPath = dlg.FileName;
                VideoPathBox.Text = VideoPath;
            }
        }

        private void Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        private string CheckValidityAuthorizationCode()
        {
            int textLength = AuthorizationCodeBox.Text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
            {
                errorString = Strings.GooglePlus_AuthorizationCode_NoCharacter;
            }
            else if (textLength > 300)
            {
                errorString = Strings.GooglePlus_AuthorizationCode_TooLong;
            }

            CanSave = textLength > 0 && (textLength <= 300);

            // If an Authorization Code is setted and the Authentication process is started we can click the CheckCode button.
            buttonCheckCode.IsEnabled = (CanSave && canEnableCheckCode);

            return errorString;
        }

        private string CheckValidityField(string text, int maxLength, string type)
        {
            int textLength = text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
            {
                errorString = string.Format(Strings.General_NoCharacter, type);
            }
            else if (textLength > maxLength)
            {
                errorString = string.Format(Strings.General_TooLong, type);
            }

            CanSave = textLength > 0 && (textLength <= maxLength);

            return errorString;
        }

        private string CheckValidityVideoPath()
        {
            int textLength = VideoPathBox.Text.Length;
            string errorString = string.Empty;
            bool fileExits = true;

            if (textLength == 0)
            {
                errorString = string.Format(Strings.General_NoCharacter, Strings.YouTube_VideoPath);
            }
            else if (textLength > 300)
            {
                errorString = string.Format(Strings.General_TooLong, Strings.YouTube_VideoPath);
            }
            else if (!File.Exists(VideoPathBox.Text))
            {
                // The file doesn't exists
                errorString = Strings.General_FileNotFound;
                fileExits = false;
            }

            CanSave = textLength > 0 && (textLength <= 300) && fileExits;

            return errorString;
        }

        private void CheckValidity()
        {
            string errorString = string.Empty;
            CanSave = true;

            errorString = CheckValidityField(VideoTitleBox.Text, 200, Strings.YouTube_VideoTitle);

            if (errorString.Equals(string.Empty))
            {
                errorString = CheckValidityField(DescriptionBox.Text, 500, Strings.YouTube_Description);
            }

            if (errorString.Equals(string.Empty))
            {
                errorString = CheckValidityVideoPath();
            }

            if (errorString.Equals(string.Empty))
            {
                errorString = CheckValidityAuthorizationCode();
            }

            if (errorString.Equals(string.Empty) && authenticationFailed)
            {
                errorString = Strings.General_AuthenticationFailed;
            }

            if (errorString.Equals(string.Empty) && !isAuthenticated)
            {
                errorString = Strings.General_NotAuthenticated;
            }

            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                CanSave = false;
            }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
