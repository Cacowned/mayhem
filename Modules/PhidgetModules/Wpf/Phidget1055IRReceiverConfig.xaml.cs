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
using MayhemWpf.UserControls;

namespace PhidgetModules.Wpf
{
    public partial class Phidget1055IRReceiveConfig : IWpfConfiguration
    {
        protected IR ir;

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

            this.DataContext = this;
            InitializeComponent();
        }

        

        public override void OnLoad()
        {
            ir.Code += new IRCodeEventHandler(ir_Code);
            ir.Attach += new AttachEventHandler(ir_Attach);
            ir.Detach += new DetachEventHandler(ir_Detach);
        }

        public override void OnClosing()
        {
            ir.Code -= ir_Code;
            ir.Attach -= ir_Attach;
            ir.Detach -= ir_Detach;
        }

        public override bool OnSave()
        {
            if (Code == null)
            {
                MessageBox.Show("You must send a code.");
                return false;
            }
            return true;
        }

        public override string Title
        {
            get
            {
                return "Phidget - IR Receiver";
            }
        }

        void ir_Code(object sender, IRCodeEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                this.Code = e.Code;
            }));
        }

        void ir_Attach(object sender, Phidgets.Events.AttachEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                this.NoReciever.Visibility = Visibility.Collapsed;
            }));
        }

        void ir_Detach(object sender, DetachEventArgs e)
        {
 	        this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                this.NoReciever.Visibility = Visibility.Visible;
            }));
        }
    }
}
