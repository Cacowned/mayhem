using System;
using System.Linq;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using Phidgets;
using Phidgets.Events;
using PhidgetModules.LowLevel;

namespace PhidgetModules.Events
{
	[DataContract]
	[MayhemModule("Phidget: IR Receiver", "Triggers when it sees a certain IR code")]
	public class Phidget1055IrReceiver : EventBase, IWpfConfigurable
	{
		[DataMember]
		private MayhemIRCode code;

		private IR ir;
		private DateTime lastSignal;

		public WpfConfiguration ConfigurationControl
		{
			get { return new Phidget1055IrReceiverConfig(code); }
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			code = ((Phidget1055IrReceiverConfig)configurationControl).Code;
		}

		public string GetConfigString()
		{
			return string.Format("IR Code 0x{0}", code);
		}

		// When we receive a code
		private void ir_Code(object sender, IRCodeEventArgs e)
		{
			if (code == null)
			{
				ErrorLog.AddError(ErrorType.Failure, "No code is set, please reconfigure");
				return;
			}

			// If the data matches,
			// Do we care about the number of times it was repeated?
			if (code.Data.SequenceEqual(e.Code.Data))
			{
				// We need to make a timeout for the IR
				TimeSpan diff = DateTime.Now - lastSignal;
				if (diff.TotalMilliseconds >= 750)
				{
					// then trigger
					Trigger();
				}

				lastSignal = DateTime.Now;
			}
		}

		protected override void OnEnabling(EnablingEventArgs e)
		{
			// If we weren't just configuring, open the sensor
			if (!e.WasConfiguring)
			{
				try
				{
					ir = PhidgetManager.Get<IR>();
				}
				catch (InvalidOperationException)
				{
					ErrorLog.AddError(ErrorType.Failure, "The IR Phidget is not attached");
					e.Cancel = true;
					return;
				}
			}

			ir.Code += ir_Code;
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			ir.Code -= ir_Code;

			// only release if we aren't going into the configuration menu
			if (!e.IsConfiguring)
			{
				PhidgetManager.Release<IR>(ref ir);
			}
		}
	}
}
