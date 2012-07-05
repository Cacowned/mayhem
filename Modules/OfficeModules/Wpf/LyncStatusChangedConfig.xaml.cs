using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using OfficeModules.Resources;

namespace OfficeModules.Wpf
{
    /// <summary>
    /// User Control for monitoring the status of a predefined user.
    /// </summary>
    public partial class LyncStatusChangedConfig : WpfConfiguration
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
        /// The status that is monitored.
        /// </summary>
        public string Status
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
        private ObservableCollection<string> statuses = null;

        /// <summary>
        /// The constructor of the LyncStatusChangedConfig class.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="status">The status</param>
        /// <param name="title">The title of the config window</param>
        public LyncStatusChangedConfig(string userId, string status, string title)
        {
            UserId = userId;
            Status = status;
            configTitle = title;

            InitializeComponent();
        }        

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
        public override void OnLoad()
        {
            UserIdBox.Text = UserId;

            if (statuses == null)
                statuses = new ObservableCollection<string>();
            else
                statuses.Clear();

            statuses.Add("Any");
            statuses.Add("Available");
            statuses.Add("Busy");
            statuses.Add("Do not disturb");
            statuses.Add("Be right back");
            statuses.Add("Off work");
            statuses.Add("Away");

            StatusComboBox.ItemsSource = statuses;

            if (Status == null || Status.Equals(""))
                StatusComboBox.SelectedIndex = 0;
            else
                StatusComboBox.SelectedItem = Status;

            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the user clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            UserId = UserIdBox.Text;
            Status = StatusComboBox.SelectedItem as string;
        }

        /// <summary>
        /// This method will check the validity of the information provided by the user.
        /// </summary>
        private void CheckValidity()
        {
            CanSave = UserIdBox.Text.Trim().Length > 0;
            textInvalid.Text = Strings.Lync_InvalidUserId;

            if (StatusComboBox.Items.Count == 0 || StatusComboBox.SelectedIndex == -1)
            {
                CanSave = false;
                textInvalid.Text = Strings.Lync_ComboBoxNoItems;
            }

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
