﻿using System.Windows;
using System.Windows.Threading;
using MayhemWpf.UserControls;
using Phidgets;
using Phidgets.Events;
using System;

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
			Code = code;

			DataContext = this;
			InitializeComponent();
		}

		public override string Title
		{
			get
			{
				return "Phidget: IR Receiver";
			}
		}

		public override void OnLoad()
		{
			Ir = PhidgetManager.Get<IR>(throwIfNotAttached: false);

			Ir.Code += ir_Code;
			Ir.Attach += ir_Attach;
			Ir.Detach += ir_Detach;

			CheckCanSave();
		}

		#region Phidget Event Handlers
		private void ir_Code(object sender, IRCodeEventArgs e)
		{
			Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
			{
				CanSave = true;
				Code = e.Code;
			}));
		}

		private void CheckCanSave()
		{
			Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
			{
				if (Ir.Attached)
				{
					NoReciever.Visibility = Visibility.Collapsed;
				}
				else
				{
					NoReciever.Visibility = Visibility.Visible;
					CanSave = false;
				}
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
			Ir.Code -= ir_Code;
			Ir.Attach -= ir_Attach;
			Ir.Detach -= ir_Detach;

			PhidgetManager.Release<IR>(ref Ir);
		}
	}
}
