using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MayhemWpf.UserControls;
using OfficeModules.Resources;

namespace OfficeModules.Wpf
{
    /// <summary>
    /// User Control for sending a file to a predefined user.
    /// </summary>
    public partial class LyncSendFileConfig : WpfConfiguration
    {
        public string UserId
        {
            get;
            private set;
        }

        /// <summary>
        /// The path of the file to be sent to the user.
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return "Lync: Send File"; }
        }

        public LyncSendFileConfig(string userId, string filename)
        {
            InitializeComponent();

            UserId = userId;
            FileName = filename;
        }

        public override void OnLoad()
        {
            LocationBox.Text = FileName;
            UserIdBox.Text = UserId;

            CheckValidity();
        }

        public override void OnSave()
        {
            FileName = LocationBox.Text;
            UserId = UserIdBox.Text;
        }

        private void CheckValidity()
        {
            string text = LocationBox.Text;

            CanSave = true;

            if (text.Length == 0)
            {
                textInvalid.Text = Strings.General_FileLocationInvalid;
                CanSave = false;
            }
            else
                if (!File.Exists(text))
                {
                    // The file doesn't exists
                    textInvalid.Text = Strings.General_FileNotFound;
                    CanSave = false;
                }
                else
                    if (UserIdBox.Text.Trim().Length == 0)
                    {
                        textInvalid.Text = Strings.Lync_InvalidUserId;
                        CanSave = false;
                    }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void BrowseFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();

            dlg.FileName = FileName;

            if (dlg.ShowDialog().Equals(DialogResult.OK))
            {
                FileName = dlg.FileName;
                LocationBox.Text = FileName;
            }
        }

        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        private void UserIdBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
