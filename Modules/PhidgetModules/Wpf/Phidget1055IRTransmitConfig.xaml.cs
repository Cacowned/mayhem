﻿using System.Windows;
using System.Windows.Threading;
using MayhemWpf.UserControls;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Reaction
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

            this.DataContext = this;
            InitializeComponent();
        }

        public override string Title
        {
            get
            {
                return "Phidget - IR Transmit";
            }
        }

        public override void OnLoad()
        {
            ir.Learn += ir_Learn;
            ir.Attach += ir_Attach;
            ir.Detach += ir_Detach;
        }

        #region Phidget Event Handlers
        void ir_Learn(object sender, IRLearnEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                this.CanSave = true;
                this.Code = e.LearnedCode.Code;
                this.CodeInfo = e.LearnedCode.CodeInfo;
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
            ir.Learn -= ir_Learn;
            ir.Attach -= ir_Attach;
            ir.Detach -= ir_Detach;
        }
    }
}
