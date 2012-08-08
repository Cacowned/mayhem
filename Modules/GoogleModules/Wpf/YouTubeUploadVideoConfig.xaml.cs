using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using GoogleModules.Resources;

namespace GoogleModules.Wpf
{
    /// <summary>
    /// User Control for setting the information about the video an user wants to upload.
    /// </summary>
    public partial class YouTubeUploadVideoConfig : GoogleAuthenticationBaseConfig
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

        private const string Scope = "https://gdata.youtube.com";

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
            Authenticate(Scope);
        }

        private void buttonCheckCode_Click(object sender, RoutedEventArgs e)
        {
            authorizationCode = AuthorizationCodeBox.Text;

            eventAuthorizationCodeEnter.Set();

            // We need to wait for the authentication to take place. If in 20 seconds the authentication doesn't take place we stop it. 
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

            if (!CheckValidityField(videoPath, Strings.YouTube_VideoPath, maxLength: 300))
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

            // The text fields of the configuration window are checked in order and if an error is found the evaluation of this expresion will stop and the error will be displayed.
            // The evaluation variable is not used but it won't compile if I don't store the result.
            bool evaluation = CheckValidityField(VideoTitleBox.Text, Strings.YouTube_VideoTitle, maxLength: 200) &&
                              CheckValidityField(DescriptionBox.Text, Strings.YouTube_Description, maxLength: 500) &&
                              CheckValidityVideoPath(VideoPathBox.Text) &&
                              CheckValidityAuthorizationCode(AuthorizationCodeBox.Text, buttonCheckCode) &&
                              CheckAuthentication();

            DisplayErrorMessage(textInvalid);
        }
    }
}
