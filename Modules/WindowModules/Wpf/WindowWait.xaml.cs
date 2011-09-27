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
    public partial class WindowWait : UserControl, WindowActionConfigControl
    {
        WindowActionWait action;

        public WindowWait(WindowActionWait action)
        {
            InitializeComponent();

            this.action = action;

            textPauseMs.Text = action.Milliseconds.ToString();
        }

        public void Save()
        {
            action.Milliseconds = int.Parse(textPauseMs.Text);
        }
    }
}
