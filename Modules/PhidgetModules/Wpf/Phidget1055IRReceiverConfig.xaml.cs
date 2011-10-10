using System.Windows;
using System.Windows.Threading;
using MayhemWpf.UserControls;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Wpf
{
    public partial class Phidget1055IrReceiverConfig : WpfConfiguration
    {
        protected IR Ir;

        public IRCode Code
        {
            get { return (IRCode)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Code.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(IRCode), typeof(Phidget1055IrReceiverConfig), new UIPropertyMetadata(null));

        public Phidget1055IrReceiverConfig(IRCode code)
        {
            Ir = InterfaceFactory.Ir;
            Code = code;

            this.DataContext = this;
            InitializeComponent();
        }

        public override string Title
        {
            get
            {
                return "Phidget - IR Receiver";
            }
        }

        public override void OnLoad()
        {
            Ir.Code += ir_Code;
            Ir.Attach += ir_Attach;
            Ir.Detach += ir_Detach;
        }

        #region Phidget Event Handlers
        private void ir_Code(object sender, IRCodeEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                CanSave = true;
                this.Code = e.Code;
            }));
        }

        private void ir_Attach(object sender, Phidgets.Events.AttachEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                this.NoReciever.Visibility = Visibility.Collapsed;
            }));
        }

        private void ir_Detach(object sender, DetachEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                this.NoReciever.Visibility = Visibility.Visible;
            }));
        }
        #endregion

        public override void OnClosing()
        {
            Ir.Code -= ir_Code;
            Ir.Attach -= ir_Attach;
            Ir.Detach -= ir_Detach;
        }
    }
}
