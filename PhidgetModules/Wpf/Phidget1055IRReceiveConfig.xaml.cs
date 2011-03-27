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
using Phidgets;
using Phidgets.Events;
using System.Windows.Threading;

namespace PhidgetModules.Wpf
{
    public partial class Phidget1055IRReceiveConfig : Window
    {
        protected IR ir;
        protected IRCodeEventHandler gotCode;

        public IRCode Code
        {
            get { return (IRCode)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Code.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(IRCode), typeof(Phidget1055IRReceiveConfig), new UIPropertyMetadata(null));

        public Phidget1055IRReceiveConfig(IRCode code)
        {
            ir = InterfaceFactory.GetIR();
            Code = code;

            gotCode = new IRCodeEventHandler(ir_Code);

            InitializeComponent();
            
        }

        void ir_Code(object sender, IRCodeEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                this.Code = e.Code;
            }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ir.Code += gotCode;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ir.Code -= gotCode;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (Code == null)
            {
                MessageBox.Show("You must send a code.");
                return;
            }
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
