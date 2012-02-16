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

        public StatusConfig()
        {
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
    }
}
