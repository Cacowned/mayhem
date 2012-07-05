using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace OfficeModules.Wpf
{
    /// <summary>
    /// User Control for selecting the user that will be monitored.
    /// </summary>
    public partial class LyncSelectUserConfig : WpfConfiguration
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
        /// The title of the user control.
        /// </summary>
        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        /// <summary>
        /// The constructor of the LyncSelectedUserConfig class.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        public LyncSelectUserConfig(string userId, string title)
        {
            UserId = userId;

            InitializeComponent();
            configTitle = title;
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
        public override void OnLoad()
        {
            UserIdBox.Text = UserId;

            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the user clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            UserId = UserIdBox.Text;
        }

        /// <summary>
        /// This method will check the validity of the information provided by the user.
        /// </summary>
        private void CheckValidity()
        {
            CanSave = UserIdBox.Text.Trim().Length > 0;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
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
