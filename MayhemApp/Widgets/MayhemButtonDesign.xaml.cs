using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MayhemApp.Business_Logic;

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
                if (b != null)
                {
                    b.GotDoubleClicked(this, e);
                }
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
