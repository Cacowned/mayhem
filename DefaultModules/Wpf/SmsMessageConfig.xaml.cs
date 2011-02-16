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

namespace DefaultModules.Wpf
{
    /// <summary>
    /// Interaction logic for SmsMessageConfig.xaml
    /// </summary>
    public partial class SmsMessageConfig : Window
    {
        public string to;
        public string subject;
        public string msg;
        public string mailServer;
        public string from;

        // TODO: This is probably a security hole
        public string password;

        public SmsMessageConfig(string to, string subject, string message, string mailserver, string from, string password) {
            InitializeComponent();

            this.to = ToBox.Text = to;
            this.subject = SubjectBox.Text = subject;
            this.msg = MsgBox.Text = message;
            this.mailServer = ServerBox.Text = mailserver;
            this.from = FromBox.Text = from;
            this.password = PasswordBox.Password = password;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e) {

            to = ToBox.Text;
            subject = SubjectBox.Text;
            msg = MsgBox.Text;
            mailServer = ServerBox.Text;
            from = FromBox.Text;
            password = PasswordBox.Password;

            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}
