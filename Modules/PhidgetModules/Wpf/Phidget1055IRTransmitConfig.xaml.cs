using System.Windows;
using System.Windows.Threading;
using MayhemWpf.UserControls;
using Phidgets;
using Phidgets.Events;
using MayhemCore;

namespace PhidgetModules.Wpf
{
    /// <summary>
    /// Interaction logic for Phidget1055IRTransmit.xaml
    /// </summary>
    public partial class Phidget1055IrTransmitConfig : WpfConfiguration
    {
        protected IR ir;

        public IRCode Code
        {
            get { return (IRCode)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Code.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(IRCode), typeof(Phidget1055IrTransmitConfig), new UIPropertyMetadata(null));

        public IRCodeInfo CodeInfo { get; private set; }

        public Phidget1055IrTransmitConfig(IRCode code, IRCodeInfo codeInfo)
        {
            ir = InterfaceFactory.Ir;
            Code = code;
            CodeInfo = codeInfo;

            DataContext = this;
            InitializeComponent();
        }

        public override string Title
        {
            get
            {
                return "Phidget: IR Transmit";
            }
        }

        public override void OnLoad()
        {
            ir.Learn += ir_Learn;
            ir.Attach += ir_Attach;
            ir.Detach += ir_Detach;
        	ir.Code += ir_Code;

			if(ir.Attached)
			{
				ir_Attach(null, null);
			}
        }

        #region Phidget Event Handlers
		
		private void ir_Code(object sender, IRCodeEventArgs e)
		{
			Logger.WriteLine("Code");
		}

        private void ir_Learn(object sender, IRLearnEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                CanSave = true;
                Code = e.LearnedCode.Code;
                CodeInfo = e.LearnedCode.CodeInfo;
            }));
        }

        private void ir_Attach(object sender, AttachEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                NoReciever.Visibility = Visibility.Collapsed;
            }));
        }

        private void ir_Detach(object sender, DetachEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                NoReciever.Visibility = Visibility.Visible;
            }));
        }
        #endregion

        public override void OnClosing()
        {
            ir.Learn -= ir_Learn;
            ir.Attach -= ir_Attach;
            ir.Detach -= ir_Detach;
        }

	}
}
