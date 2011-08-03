using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using MayhemDefaultStyles.UserControls;

namespace DefaultModules.Wpf
{
    /// <summary>
    /// Interaction logic for SmsMessageConfig.xaml
    /// </summary>
    public partial class SmsMessageConfig : IWpfConfig
    {
        public string to;
        public string msg;

        public string carrierString;

        protected Dictionary<string, string> carriers;

        // TODO: This is probably a security hole
        public string password;

        public SmsMessageConfig(string to, string message, Dictionary<string, string> carriers)
        {
            this.to = to;
            this.msg = message;
            this.carriers = carriers;

            InitializeComponent();
        }

        protected override void OnInitialized(System.EventArgs e)
        {
            base.OnInitialized(e);

            ToBox.Text = to;
            MsgBox.Text = msg;
            Carrier.ItemsSource = this.carriers.Keys;

            Carrier.SelectedIndex = 0;
        }

        public override string Title
        {
            get { return "SMS Message"; }
        }

        public override bool OnSave()
        {
            to = ToBox.Text;
            msg = MsgBox.Text;
            carrierString = carriers[Carrier.SelectedValue.ToString()];
            return true;
        }

        public override void OnCancel()
        {
        }
    }
}
