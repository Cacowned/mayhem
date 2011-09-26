using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using Phidgets;
using PhidgetModules.Wpf;
using System.Windows;
using Phidgets.Events;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace PhidgetModules.Events
{
    [DataContract]
    //[MayhemModule("Phidget: Digital Input", "Triggers on a digital input")]
    public class DigitalInput : EventBase, IWpfConfigurable
    {
        #region Configuration
        // Which index do we want to be looking at?
        [DataMember]
        private int Index;

        // Toggle when it goes on, or when it goes off?
        [DataMember]
        private bool OnWhenOn;

        #endregion

        // The interface kit we are using for the sensors
        private InterfaceKit ifKit;

        private InputChangeEventHandler inputChangeHandler;

        protected override void Initialize()
        {
            this.ifKit = InterfaceFactory.Interface;
            inputChangeHandler = new InputChangeEventHandler(InputChanged);

            Index = 0;
            OnWhenOn = true;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new PhidgetDigitalInputConfig(ifKit, Index, OnWhenOn); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            Index = ((PhidgetDigitalInputConfig)configurationControl).Index;
            OnWhenOn = ((PhidgetDigitalInputConfig)configurationControl).OnWhenOn;
        }

        public override void SetConfigString()
        {
            string type = "turns on";
            if (!OnWhenOn)
            {
                type = "turns off";
            }

            ConfigString = String.Format("Triggers when input #{0} {1}", Index, type);
        }

        // The input has changed, do the work here
        protected void InputChanged(object sender, InputChangeEventArgs ex)
        {
            // If e.CurrentValue is true, then it used to be false.
            // Trigger when appropriate

            // We are dealing with the right input
            if (ex.Index == Index)
            {
                // If its true and we turn on when it turns on
                // then trigger
                if (ex.Value == true && OnWhenOn)
                {
                    Trigger();
                }
                // otherwise, if it its off, and we trigger
                // when it turns off, then trigger
                else if (ex.Value == false && !OnWhenOn)
                {
                    Trigger();
                }
            }
        }


        public override bool Enable()
        {
            ifKit.InputChange += inputChangeHandler;

            return true;
        }

        public override void Disable()
        {
            if (ifKit != null)
            {
                ifKit.InputChange -= inputChangeHandler;
            }

        }
    }
}
