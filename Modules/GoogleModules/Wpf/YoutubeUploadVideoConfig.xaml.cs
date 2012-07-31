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
using Google.YouTube;
using GoogleModules.Resources;
using MayhemWpf.UserControls;
using MayhemCore;
using Google.GData.Extensions.MediaRss;
using Google.GData.YouTube;
using Google.GData.Extensions.Location;
using Google.GData.Client;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// Interaction logic for YoutubeUploadVideoConfig.xaml
    /// </summary>
    public partial class YoutubeUploadVideoConfig : WpfConfiguration
    {
        public string Username
        {
            get;
            private set;
        }

        public string Password
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

        public string Keywords
        {
            get;
            private set;
        }

        public string FilePath
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;
        private bool authenticationFailed;
        private bool isAuthenticated;

        public YoutubeUploadVideoConfig()
        {
            InitializeComponent();

            configTitle = Strings.YoutubeUploadVideo_Title;
            UsernameBox.Text = "ttest1112@gmail.com";
            PasswordBox.Password = "macarazena12";
        }

        private void buttonAuthenticate_Click(object sender, RoutedEventArgs e)
        {
            Username = UsernameBox.Text;
            Password = PasswordBox.Password;

           
            try
            {
                
                YouTubeRequestSettings settings = new YouTubeRequestSettings(Strings.Youtube_ProductName, Strings.Youtube_DeveloperKey, Username, Password);
                YouTubeRequest request = new YouTubeRequest(settings);
                Uri uri = settings.ClientLoginHandler;

               

                string token = settings.Token;
                string a = request.Settings.OAuthUser;
                Video newVideo = new Video();
                newVideo.Title = "My Test Movie";
                newVideo.Tags.Add(new MediaCategory("Autos", YouTubeNameTable.CategorySchema));
                newVideo.Keywords = "cars, funny";
                newVideo.Description = "My description";
                newVideo.YouTubeEntry.Private = false;
                newVideo.Tags.Add(new MediaCategory("mydevtag, anotherdevtag", YouTubeNameTable.DeveloperTagSchema));
                newVideo.YouTubeEntry.Location = new GeoRssWhere(37, -122);
                // alternatively, you could just specify a descriptive string
                // newVideo.YouTubeEntry.setYouTubeExtension("location", "Mountain View, CA");
                // newVideo.YouTubeEntry.MediaSource = new MediaFileSource("d:\\a.avi",  "video/quicktime");

                Video createdVideo = request.Upload(newVideo);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private void CheckValidityLogin()
        {
            string errorString = string.Empty;
            CanSave = true;

            errorString = CheckValidityField(UsernameBox.Text, 100, Strings.Youtube_Username);

            if (errorString.Equals(string.Empty))
            {
                errorString = CheckValidityField(PasswordBox.Password, 100, Strings.Youtube_Password);
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

        private void UsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PasswordBox_TextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void TitleBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DescriptionBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CategoryBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void KeywordsBox_TextChanged(object sender, TextChangedEventArgs e)
        {

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
    }
}
