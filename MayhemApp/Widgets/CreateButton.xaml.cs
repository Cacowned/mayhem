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

namespace MayhemApp
{
    /// <summary>
    /// Interaction logic for CreateButton.xaml
    /// </summary>
    public partial class CreateButton : UserControl
    {
        public CreateButton()
        {
            InitializeComponent();
        }

        public delegate void ClickHandler(object sender, EventArgs e);
        public event EventHandler Click;

        
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            (((sender as Button).Parent as Grid).Parent as CreateButton).Click(sender, e);

            e.Handled = false;
        }
    }
}
