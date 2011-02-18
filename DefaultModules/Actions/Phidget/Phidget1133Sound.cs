using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phidgets;         //needed for the interfacekit class and the phidget exception class
using Phidgets.Events;
using MayhemCore.ModuleTypes;
using MayhemCore;
using System.Runtime.Serialization;  //needed for the event handling classes

namespace DefaultModules.Actions.Phidget
{
    [Serializable]
    public class Phidget1133Sound : ActionBase, IWpf, ISerializable
    {
        protected InterfaceKit ifKit;
        protected int decibles;

        public Phidget1133Sound()
            : base("Phidget [1133]: Sound", "Triggers at a certain sound level") {
            hasConfig = true;

            SetUp();
        }

        protected void SetConfigString() {
            ConfigString = String.Format("{0} dB", decibles);
        }

        protected void SetUp() {
            ifKit = new InterfaceKit();
            
        }

        public void WpfConfig() {
            /*
            var window = new TimerConfig(hours, minutes, seconds);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (window.ShowDialog() == true) {

                this.hours = window.hours;
                this.minutes = window.minutes;
                this.seconds = window.seconds;

                SetInterval();
            }
             * */
        }

        /*
        private void myTimer_Elapsed(object sender, ElapsedEventArgs e) {
            base.OnActionActivated();
        }*/

        public override void Enable() {
            base.Enable();
        }

        public override void Disable() {
            base.Disable();
        }

        #region Serialization

        public Phidget1133Sound(SerializationInfo info, StreamingContext context)
            : base(info, context) {

            SetUp();
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
        }
        #endregion
    }
}
