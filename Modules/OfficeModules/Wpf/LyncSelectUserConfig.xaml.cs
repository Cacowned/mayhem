using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace OfficeModules.Wpf
{
    public partial class LyncSelectUserConfig : WpfConfiguration
    {
        public string UserId
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return "Select User"; }
        }

        public LyncSelectUserConfig(string userId)
        {
            UserId = userId;

            InitializeComponent();
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
