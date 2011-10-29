using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using Phidgets;

namespace PhidgetModules.Reaction
{
    [DataContract]
    [MayhemModule("Phidget: Adv. Servo", "Controls a servo")]
    public class Phidget1066AdvServo : ReactionBase, IWpfConfigurable
    {
        // Instance of the servo class
        private static AdvancedServo advServo;

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

        protected override void OnAfterLoad()
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
        }

        public override void Perform()
        {
            // If we have a servo, use the first one. 
            // TODO: This should be configurable
            if (advServo.servos.Count >= 1)
            {
                advServo.servos[0].Position = position;
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new Phidget1066AdvServoConfig(advServo, index, servoType, position); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            servoType = (configurationControl as Phidget1066AdvServoConfig).ServoType;
            position = (configurationControl as Phidget1066AdvServoConfig).Position;
        }

        public string GetConfigString()
        {
            return string.Format("Move to {0}", position.ToString("0.##"));
        }
    }
}
