using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using PhidgetModules.Wpf;
using System.Windows;
using System;
using Phidgets;

namespace PhidgetModules.Reaction
{
	[Serializable]
	public class Phidget1066AdvServo : ReactionBase, IWpf, ISerializable//, IDisposable
	{
		// Instance of the servo class
		protected static AdvancedServo advServo;
		
		// count how many instances of this class we have
		// so we know when to close our connection to the Servo
		//protected static int instances = 0;

		// This is the position that this reaction instance wants to 
		// move the servo to
		protected double position;

		// Motor Type
		protected ServoServo.ServoType servoType;

		// The servo index we are talking to
		protected int index;

		//private bool disposed = false; // to detect redundant calls

		public Phidget1066AdvServo()
			: base("Phidget-1066: Adv. Servo", "Controls a servo") {
			
			position = 50;

			// This is the one we have, so we are just defaulting to it
			servoType = ServoServo.ServoType.HITEC_HS322HD;
				
			Setup();
		}

		protected void Setup() {
			hasConfig = true;

			// Increment the number of instances we have of this class
			//instances++;

			// We are currently only using a single servo. In the future,
			// this should be changed
			index = 0;

			// Only maintain one instance of the servo 
			// for all the AdvServo classes
			if (advServo == null) {
				advServo = new AdvancedServo();
				advServo.open();

                foreach (AdvancedServoServo servo in advServo.servos)
                {
                    servo.Engaged = true;
                    servo.SpeedRamping = false;
                }

			}

			SetConfigString();
		}

		public override void Perform() {
            //advServo.servos[0].Engaged = true;
			
			// If we have a servo, use the first one. 
			// TODO: This should be configurable
			if (advServo.servos.Count >= 1) {
				advServo.servos[0].Position = position;
			}
            
		}

		public void WpfConfig() {
			var window = new Phidget1066AdvServoConfig(advServo, index, servoType, position);
			window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

			if (window.ShowDialog() == true) {
				servoType = window.ServoType;
				position = window.Position;

				SetConfigString();
			}
			
		}


		protected void SetConfigString() {
			ConfigString = String.Format("Move to {0}", position.ToString("0.##"));
		}


		#region Serialization
		
		public Phidget1066AdvServo(SerializationInfo info, StreamingContext context)
			: base(info, context) {

			index = info.GetInt32("Index");
			position = info.GetDouble("Position");
			servoType = (ServoServo.ServoType)info.GetValue("ServoType", typeof(ServoServo.ServoType));

			Setup();
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);

			info.AddValue("Index", index);
			info.AddValue("Position", position);
			info.AddValue("ServoType", servoType);

		}
		
		#endregion

		
	}
}
