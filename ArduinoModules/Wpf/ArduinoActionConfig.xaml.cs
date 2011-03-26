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

namespace ArduinoModules.Wpf
{
    /// <summary>
    /// Interaction logic for ArduinoActionConfig.xaml
    /// </summary>
    public partial class ArduinoActionConfig : Window
    {
        public ArduinoActionConfig()
        {
            InitializeComponent();
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {

            DialogResult = true; 
        }
    }
}
