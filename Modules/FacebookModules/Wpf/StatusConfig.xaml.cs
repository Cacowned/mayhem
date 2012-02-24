using FacebookModules.Wpf.UserControls;

namespace FacebookModules.Wpf
{
    /// <summary>
    /// Interaction logic for StatusConfig.xaml
    /// </summary>
    public partial class StatusConfig : FacebookConfigControl
    {
        public string StatusProp
        {
            get;
            private set;
        }

        public bool CanSave
        {
            get;
            private set;
        }

        public StatusConfig()
        {
            CanSave = true;
            InitializeComponent();
        }

        public override string Title
        {
            get
            {
                return "Facebook - Status Update";
            }
        }

        public override void OnSave()
        {
            StatusProp = statusText.Text;
        }

        private void StatusText_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CanSave = statusText.Text.Length > 0;
        }
    }
}
