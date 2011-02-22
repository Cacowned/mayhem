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
    /// Interaction logic for DebugMessageConfig.xaml
    /// </summary>
    public partial class DebugMessageConfig : Window
    {
        public string message;

        public DebugMessageConfig(string message) {
            this.message = message;
            InitializeComponent();

            MessageText.Text = this.message;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e) {
            if (MessageText.Text.Trim().Length == 0) {
                MessageBox.Show("You must provide a message");
                return;
            }
            message = MessageText.Text.Trim();
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}
