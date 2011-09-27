using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using PhidgetModules.Wpf;
using System.Windows;
using System;
using Phidgets;
using MayhemWpf.UserControls;

namespace PhidgetModules.Reaction
{
    [DataContract]
    [MayhemModule("Phidget: Adv. Servo", "Controls a servo")]
    public class Phidget1066AdvServo : ReactionBase, IWpfConfigurable
    {
        #region Configuration

        // Motor Type
        [DataMember]
        private ServoServo.ServoType ServoType;

        // The servo index we are talking to
        [DataMember]
        private int Index;

        // This is the position that this reaction instance wants to 
        // move the servo to
        [DataMember]
        private double Position;

        #endregion;
        // Instance of the servo class
        private static AdvancedServo advServo;

        protected override void Initialize()
        {
            // Only maintain one instance of the servo 
            // for all the AdvServo classes
            if (advServo == null)
            {
                advServo = new AdvancedServo();
                advServo.open();

                foreach (AdvancedServoServo servo in advServo.servos)
                {
                    servo.Engaged = true;
                    servo.SpeedRamping = false;
                }

            }

            Position = 50;
            Index = 0;

            // This is the one we have, so we are just defaulting to it
            ServoType = ServoServo.ServoType.HITEC_HS322HD;
        }

        public override void Perform()
        {
            //advServo.servos[0].Engaged = true;

            // If we have a servo, use the first one. 
            // TODO: This should be configurable
            if (advServo.servos.Count >= 1)
            {
                advServo.servos[0].Position = Position;
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new Phidget1066AdvServoConfig(advServo, Index, ServoType, Position); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            ServoType = (configurationControl as Phidget1066AdvServoConfig).ServoType;
            Position = (configurationControl as Phidget1066AdvServoConfig).Position;
        }

        public string GetConfigString()
        {
            return String.Format("Move to {0}", Position.ToString("0.##"));
        }
    }
}
