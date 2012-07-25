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
        public string UserId
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        public LyncSelectUserConfig(string userId, string title)
        {
            UserId = userId;

            InitializeComponent();
            configTitle = title;
        }

        public override void OnLoad()
        {
            UserIdBox.Text = UserId;

            CheckValidity();
        }

        public override void OnSave()
        {
            UserId = UserIdBox.Text;
        }

        private void CheckValidity()
        {
            CanSave = UserIdBox.Text.Trim().Length > 0;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void UserIdBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }        
    }
}
