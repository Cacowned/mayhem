using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using Phidgets;
using System;

namespace PhidgetModules.Reaction
{
	[DataContract]
	[MayhemModule("Phidget: Adv. Servo", "Controls a servo")]
	public class Phidget1066AdvServo : ReactionBase, IWpfConfigurable
	{
		// Instance of the servo class
		private AdvancedServo advServo;

		// Motor Type
		[DataMember]
		private ServoServo.ServoType servoType;

		// The servo index we are talking to
		[DataMember]
		private int index;

		// This is the position that this reaction instance wants to 
		// move the servo to
		[DataMember]
		private double position;

		protected override void OnLoadDefaults()
		{
			position = 50;
			index = 0;

			// This is the one we have, so we are just defaulting to it
			servoType = ServoServo.ServoType.HITEC_HS322HD;
		}

		protected override void OnEnabling(EnablingEventArgs e)
		{
			if (!e.WasConfiguring)
			{
				try
				{
					advServo = PhidgetManager.Get<AdvancedServo>();
					foreach (AdvancedServoServo servo in advServo.servos)
					{
						servo.Engaged = true;
						servo.SpeedRamping = false;
					}
				}
				catch (InvalidOperationException)
				{
					ErrorLog.AddError(ErrorType.Failure, "The Phidget servo controller is not attached");
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
				PhidgetManager.Release(ref advServo);
			}
		}

		public override void Perform()
		{
			if (!advServo.Attached || advServo.servos.Count == 0)
			{
				ErrorLog.AddError(ErrorType.Failure, "There is no Phidget servo attached");
				return;
			}

			// If we have a servo, use the first one. 
			// TODO: This should be configurable

			advServo.servos[0].Engaged = true;
			advServo.servos[0].SpeedRamping = false;
			advServo.servos[0].Position = position;
		}

		public WpfConfiguration ConfigurationControl
		{
			get { return new Phidget1066AdvServoConfig(index, servoType, position); }
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			var config = configurationControl as Phidget1066AdvServoConfig;
			servoType = config.ServoType;
			position = config.Position;
		}

		public string GetConfigString()
		{
			return string.Format("Move to {0}", position.ToString("0.##"));
		}
	}
}
