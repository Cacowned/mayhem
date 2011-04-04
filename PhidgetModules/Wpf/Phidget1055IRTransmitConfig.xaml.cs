using System.Windows;
using System.Windows.Threading;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Reaction
{
    /// <summary>
    /// Interaction logic for Phidget1055IRTransmit.xaml
    /// </summary>
    public partial class Phidget1055IRTransmitConfig : Window
    {
        protected IR ir;
        protected IRLearnEventHandler gotCode;

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
            ir = InterfaceFactory.GetIR();
            Code = code;
            CodeInfo = codeInfo;

            gotCode = new IRLearnEventHandler(ir_Learn);

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ir.Learn += gotCode;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ir.Learn -= gotCode;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (Code == null || CodeInfo == null)
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
