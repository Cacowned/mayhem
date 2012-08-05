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

namespace GoogleModules.Wpf
{
    /// <summary>
    /// User Control for setting the information about the video an user wants to upload.
    /// </summary>
    public partial class YouTubeUploadVideoConfig : GoogleBaseConfig
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

        private bool CheckValidityVideoPath(string videoPath)
        {
            errorString = string.Empty;

            if (!CheckValidityField(videoPath, 300, Strings.YouTube_VideoPath))
            {
                return false;
            }

            if (!File.Exists(VideoPathBox.Text))
            {
                // The file doesn't exists
                errorString = Strings.General_FileNotFound;
                return false;
            }

            return true;
        }

        private void CheckValidity()
        {
            errorString = string.Empty;
            CanSave = true;

            if (!CheckValidityField(VideoTitleBox.Text, 200, Strings.YouTube_VideoTitle))
            {
                DisplayErrorMessage(textInvalid);
                return;
            }

            if (!CheckValidityField(DescriptionBox.Text, 500, Strings.YouTube_Description))
            {
                DisplayErrorMessage(textInvalid);
                return;
            }

            if (!CheckValidityVideoPath(VideoPathBox.Text))
            {
                DisplayErrorMessage(textInvalid);
                return;
            }

            if (!CheckValidityField(AuthorizationCodeBox.Text, 200, Strings.General_AuthorizationCode))
            {
                DisplayErrorMessage(textInvalid);
                buttonCheckCode.IsEnabled = false;
                return;
            }
            else
            {
                // If the authorization code is valid and the other conditions are satisfied we can enable the CheckCode button
                buttonCheckCode.IsEnabled = canEnableCheckCode;
            }

            if (authenticationFailed)
            {
                errorString = Strings.General_AuthenticationFailed;
                DisplayErrorMessage(textInvalid);
                return;
            }

            if (!isAuthenticated)
            {
                errorString = Strings.General_NotAuthenticated;
                DisplayErrorMessage(textInvalid);
                return;
            }

            // If no error was found we call this method to enable the Save button and hide the error text block.
            DisplayErrorMessage(textInvalid);
        }
    }
}
