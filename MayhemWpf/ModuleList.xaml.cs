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
using System.Windows.Shapes;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Collections;

namespace MayhemWpf
{
    /// <summary>
    /// Interaction logic for ActionList.xaml
    /// </summary>
    public partial class ModuleList : Window
    {



        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ModuleList), new UIPropertyMetadata(string.Empty));


        public ModuleList(IEnumerable list, string headerText)
        {
            Text = headerText;
            InitializeComponent();
            
            ModulesList.ItemsSource = list;
        }

        private void ChooseButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
