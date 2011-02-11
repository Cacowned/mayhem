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

namespace MayhemWpf
{
    /// <summary>
    /// Interaction logic for ReactionList.xaml
    /// </summary>
    public partial class ReactionList : Window
    {
        Mayhem<ICli> mayhem;

        public ReactionList(Mayhem<ICli> mayhem)
        {
            InitializeComponent();
            this.mayhem = mayhem;

            ReactionsList.ItemsSource = mayhem.ReactionList;
        }

        private void ChooseButtonClick(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}
