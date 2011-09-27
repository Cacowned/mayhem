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

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for WindowActionControl.xaml
    /// </summary>
    public partial class WindowActionControl : UserControl
    {
        private int index;
        public int Index
        {
            get { return index; }
            set 
            {
                index = value;
                textIndex.Text = index.ToString();
            }
        }

        private UserControl config;
        public UserControl Config
        {
            get { return config; }
            set { config = value; }
        }

        // Using a DependencyProperty as the backing store for TagID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfigProperty =
            DependencyProperty.Register("Config", typeof(UserControl), typeof(WindowActionControl), new UIPropertyMetadata(null));

        public event EventHandler Deleted;

        public WindowActionControl(UserControl config)
        {
            this.Config = config;
            InitializeComponent();
            control1.Content = config;
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Deleted != null)
            {
                Deleted(this, e);
            }
        }
    }
}
