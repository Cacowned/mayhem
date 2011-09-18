using System.Windows;
using System.Windows.Threading;
using Phidgets;
using Phidgets.Events;
using MayhemWpf.UserControls;

namespace PhidgetModules.Reaction
{
    /// <summary>
    /// Interaction logic for Phidget1055IRTransmit.xaml
    /// </summary>
    public partial class Phidget1055IRTransmitConfig : IWpfConfiguration
    {
        protected IR ir;

        public IRCode Code
        {
            get { return (IRCode)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Code.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(IRCode), typeof(Phidget1055IRTransmitConfig), new UIPropertyMetadata(null));

        public IRCodeInfo CodeInfo
        {
            get { return (IRCodeInfo)GetValue(CodeInfoProperty); }
            set { SetValue(CodeInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CodeInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeInfoProperty =
            DependencyProperty.Register("CodeInfo", typeof(IRCodeInfo), typeof(Phidget1055IRTransmitConfig), new UIPropertyMetadata(null));

        public Phidget1055IRTransmitConfig(IRCode code, IRCodeInfo codeInfo)
        {
            ir = InterfaceFactory.IR;
            Code = code;
            CodeInfo = codeInfo;

            InitializeComponent();
        }

        void ir_Learn(object sender, IRLearnEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                this.Code = e.LearnedCode.Code;
                this.CodeInfo = e.LearnedCode.CodeInfo;
            }));
        }

        public override void OnLoad()
        {
            ir.Learn += ir_Learn;
        }

        public override void OnClosing()
        {
            ir.Learn -= ir_Learn;
        }

        public override bool OnSave()
        {
            if (Code == null || CodeInfo == null)
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
                return "Phidget - IR Transmit";
            }
        }
    }
}
