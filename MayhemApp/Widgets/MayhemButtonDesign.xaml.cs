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
using System.Diagnostics;

namespace MayhemApp
{
    /// <summary>
    /// Interaction logic for MayhemBlueButton.xaml
    /// </summary>
    public partial class MayhemButtonControl : UserControl
    {
        public MayhemConnectionItem connectionItem = null;

        public MayhemButtonControl()
        {
            InitializeComponent();

            MayhemButton b = this.DataContext as MayhemButton;

            //b.control = this;


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MayhemButton b = this.DataContext as MayhemButton;
            b.control = this;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Trace.WriteLine("BlueButton has MouseUP Event");
                MayhemButton b = this.DataContext as MayhemButton;
               // Debug.WriteLine("Assigned MayhemButton Text : " + b.Text);
                if (b != null)
                    b.GotDoubleClicked(this, e);
                else if (connectionItem != null)
                {
                    connectionItem.OnDoubleClick(this, e);
                }

            }
         }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

       
    }
}
