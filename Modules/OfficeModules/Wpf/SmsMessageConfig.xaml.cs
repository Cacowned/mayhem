using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MayhemWpf.UserControls;

namespace OfficeModules.Wpf
{
    /// <summary>
    /// Interaction logic for SmsMessageConfig.xaml
    /// </summary>
    public partial class SmsMessageConfig : IWpfConfiguration
    {
        public string To;
        public string Message;
        public string CarrierString;

        protected Dictionary<string, string> carriers;

        // TODO: This is probably a security hole
        public string password;

        public SmsMessageConfig(string to, string message, Dictionary<string, string> carriers)
        {
            this.To = to;
            this.Message = message;
            this.carriers = carriers;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            ToBox.Text = To;
            MsgBox.Text = Message;
            Carrier.ItemsSource = this.carriers.Keys;

            Carrier.SelectedIndex = 0;
        }

        public override string Title
        {
            get { return "SMS Message"; }
        }

        public override bool OnSave()
        {
            To = ToBox.Text;
            Message = MsgBox.Text;
            CarrierString = carriers[Carrier.SelectedValue.ToString()];
            return true;
        }

        public override void OnCancel()
        {
        }
    }
}
