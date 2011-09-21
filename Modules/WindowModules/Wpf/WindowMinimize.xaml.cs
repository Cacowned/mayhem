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
    public partial class WindowMinimize : UserControl, WindowActionConfigControl
    {
        WindowActionMinimize action;

        public WindowMinimize(WindowActionMinimize action)
        {
            InitializeComponent();

            this.action = action;
        }

        public void Save()
        {
        }
    }
}
