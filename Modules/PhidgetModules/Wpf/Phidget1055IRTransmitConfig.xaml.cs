﻿using System.Windows;
using System.Windows.Threading;
using MayhemWpf.UserControls;
using Phidgets;
using Phidgets.Events;
using MayhemCore;
using System;
using PhidgetModules.LowLevel;

namespace PhidgetModules.Wpf
{
    /// <summary>
    /// Interaction logic for Phidget1055IRTransmit.xaml
    /// </summary>
    public partial class Phidget1055IrTransmitConfig : WpfConfiguration
    {
        protected IR Ir;

        public MayhemIRCode Code
        {
			get { return (MayhemIRCode)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Code.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(MayhemIRCode), typeof(Phidget1055IrTransmitConfig), new UIPropertyMetadata(null));

        public MayhemIRCodeInfo CodeInfo { get; private set; }

        public Phidget1055IrTransmitConfig(MayhemIRCode code, MayhemIRCodeInfo codeInfo)
        {
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
			Ir = PhidgetManager.Get<IR>(throwIfNotAttached: false);

            Ir.Learn += ir_Learn;
            Ir.Attach += ir_Attach;
            Ir.Detach += ir_Detach;
        	Ir.Code += ir_Code;

        	CheckCanSave();
        }

		private void CheckCanSave()
		{
			Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
			{
				if (Ir.Attached)
				{
					// Only enable saving if we have a code already
					if (Code != null && CodeInfo != null)
					{
						CanSave = true;
					}

					NoReciever.Visibility = Visibility.Collapsed;
				}
				else
				{
					NoReciever.Visibility = Visibility.Visible;
					CanSave = false;
				}
			}));
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
                Code = new MayhemIRCode(e.LearnedCode.Code);
                CodeInfo = new MayhemIRCodeInfo(e.LearnedCode.CodeInfo);
            }));
        }

        private void ir_Attach(object sender, AttachEventArgs e)
        {
			CheckCanSave();
        }

        private void ir_Detach(object sender, DetachEventArgs e)
        {
			CheckCanSave();
        }
        #endregion

        public override void OnClosing()
        {
            Ir.Learn -= ir_Learn;
            Ir.Attach -= ir_Attach;
            Ir.Detach -= ir_Detach;
			Ir.Code -= ir_Code;

			PhidgetManager.Release(ref Ir);
        }
	}
}
