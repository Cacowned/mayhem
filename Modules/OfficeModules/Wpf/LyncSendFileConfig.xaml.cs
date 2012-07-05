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
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public string UserId
        {
            get;
            private set;
        }

        /// <summary>
        /// The path of the file.
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }        

        /// <summary>
        /// The title of the user control.
        /// </summary>
        public override string Title
        {
            get { return "Lync: Send File"; }
        }

        /// <summary>
        /// The constructor of the LyncSendFileConfig class.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="filename">The path of the file</param>
        public LyncSendFileConfig(string userId, string filename)
        {
            InitializeComponent();

            UserId = userId;
            FileName = filename;
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
        public override void OnLoad()
        {
            LocationBox.Text = FileName;
            UserIdBox.Text = UserId;

            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the user clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            FileName = LocationBox.Text;
            UserId = UserIdBox.Text;
        }

        /// <summary>
        /// This method will check the validity of the information provided by the user.
        /// </summary>
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

        /// <summary>
        /// This method will let the user select the file that wants to be sent.
        /// </summary>
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

        /// <summary>
        /// This method will be called when the text from the LocationBox changes.
        /// </summary>
        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the text from the UserIdBox changes.
        /// </summary>
        private void UserIdBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
