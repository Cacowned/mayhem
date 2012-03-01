using FacebookModules.Wpf.UserControls;
using System.Windows;
using System.Runtime.Serialization;

namespace FacebookModules.Wpf
{
    /// <summary>
    /// Interaction logic for StatusConfig.xaml
    /// </summary> 
    [DataContract]
    public partial class StatusConfig : FacebookConfigControl
    {
        public string StatusProp
        {
            get;
            private set;
        }

        public StatusConfig(string myStatus)
        {
            StatusProp = myStatus;
            InitializeComponent();
        }

        public override void OnLoad()
        {
            statusText.Text = StatusProp;
        }

        public override string Title
        {
            get
            {
                return "Facebook: Status Update";
            }
        }

        public override void OnSave()
        {
            StatusProp = statusText.Text;
        }

        private void StatusText_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CanSave = statusText.Text.Length > 0;
            Validate();
        }
    }
}
