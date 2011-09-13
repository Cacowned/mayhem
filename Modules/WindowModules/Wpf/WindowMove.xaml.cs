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
using WindowModules.Actions;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for WindowMove.xaml
    /// </summary>
    public partial class WindowMove : UserControl, WindowActionConfigControl
    {
        WindowActionMove action;

        public WindowMove(WindowActionMove action)
        {
            InitializeComponent();

            this.action = action;

            textBoxMoveX.Text = action.X.ToString();
            textBoxMoveY.Text = action.Y.ToString();
        }

        public void Save()
        {
            action.X = int.Parse(textBoxMoveX.Text);
            action.Y = int.Parse(textBoxMoveY.Text);
        }
    }
}
