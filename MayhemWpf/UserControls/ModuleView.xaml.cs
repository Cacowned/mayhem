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
using MayhemCore;

namespace MayhemWpf
{
    /// <summary>
    /// Interaction logic for ModuleView.xaml
    /// </summary>
    public partial class ModuleView : UserControl
    {




        public ModuleBase Module {
            get { return (ModuleBase)GetValue(ModuleProperty); }
            set { SetValue(ModuleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Module.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModuleProperty =
            DependencyProperty.Register("Module", typeof(ModuleBase), typeof(ModuleView), new UIPropertyMetadata(null));



        public ModuleView() {
            InitializeComponent();
        }
    }
}
