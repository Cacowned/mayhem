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
    /// Interaction logic for TrashButton.xaml
    /// </summary>
    /// 

    public partial class TrashButton : UserControl
    {

        public delegate void ButtonClickHandler(object sender, EventArgs e);
        public event ButtonClickHandler OnButtonClick;

        public TrashButton()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (OnButtonClick != null)
            {
                OnButtonClick(this, new EventArgs());
            }
        }
    }
}
