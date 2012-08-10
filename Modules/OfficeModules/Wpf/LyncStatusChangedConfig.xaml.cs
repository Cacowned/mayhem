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
        public string UserId
        {
            get;
            private set;
        }

        public string Status
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;
        private ObservableCollection<string> statuses = null;

        public LyncStatusChangedConfig(string userId, string status, string title)
        {
            UserId = userId;
            Status = status;
            configTitle = title;

            InitializeComponent();
        }

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

        public override void OnSave()
        {
            UserId = UserIdBox.Text;
            Status = StatusComboBox.SelectedItem as string;
        }

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

        private void UserIdBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
