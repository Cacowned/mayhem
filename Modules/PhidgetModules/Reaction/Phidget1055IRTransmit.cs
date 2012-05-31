using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using Phidgets;
using System;
using PhidgetModules.LowLevel;

namespace PhidgetModules.Reaction
{
	[DataContract]
	[MayhemModule("Phidget: IR Transmit", "Sends an IR code")]
	public class Phidget1055IrTransmit : ReactionBase, IWpfConfigurable
	{
		// This is the code and code information that we will transmit
		[DataMember]
		private MayhemIRCode code;

		[DataMember]
		private MayhemIRCodeInfo codeInfo;

		private IR ir;

		public WpfConfiguration ConfigurationControl
		{
			get { return new Phidget1055IrTransmitConfig(code, codeInfo); }
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			var config = configurationControl as Phidget1055IrTransmitConfig;
			code = config.Code;
			codeInfo = config.CodeInfo;
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
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			// only release if we aren't going into the configuration menu
			if (!e.IsConfiguring)
			{
				PhidgetManager.Release<IR>(ref ir);
			}
		}

		public string GetConfigString()
		{
			return string.Format("IR Code 0x{0}", code);
		}

		public override void Perform()
		{
			if (!ir.Attached)
			{
				ErrorLog.AddError(ErrorType.Failure, "The Phidget IR transmitter is not attached");
				return;
			}

			if (code != null && codeInfo != null)
			{
				ir.transmit(code.IRCode, codeInfo.IRCodeInfo);
				ir.transmitRepeat();
			}
		}
	}
}
