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
        private string text;

        public ModuleList(IEnumerable list, string headerText)
        {
            text = headerText;
            InitializeComponent();
            
            ModulesList.ItemsSource = list;
        }

        protected override void OnInitialized(EventArgs e) {
            base.OnInitialized(e);

            HeaderText.Text = this.text;
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
