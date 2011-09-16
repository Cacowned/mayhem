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
        private int Index
        {
            get;
            set;
        }

        // Toggle when it goes on, or when it goes off?
        [DataMember]
        private bool OnWhenOn
        {
            get;
            set;
        }

        #endregion

        // The interface kit we are using for the sensors
        protected InterfaceKit ifKit;

        protected InputChangeEventHandler inputChangeHandler;

        protected override void Initialize()
        {
            base.Initialize();

            this.ifKit = InterfaceFactory.GetInterface();
            inputChangeHandler = new InputChangeEventHandler(InputChanged);

            Index = 0;
            OnWhenOn = true;
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { return new PhidgetDigitalInputConfig(ifKit, Index, OnWhenOn); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
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
        protected void InputChanged(object sender, InputChangeEventArgs e)
        {
            // If e.value is true, then it used to be false.
            // Trigger when appropriate

            // We are dealing with the right input
            if (e.Index == Index)
            {
                // If its true and we turn on when it turns on
                // then trigger
                if (e.Value == true && OnWhenOn)
                {
                    OnEventActivated();
                }
                // otherwise, if it its off, and we trigger
                // when it turns off, then trigger
                else if (e.Value == false && !OnWhenOn)
                {
                    OnEventActivated();
                }
            }
        }


        public override void Enable()
        {
            base.Enable();
            ifKit.InputChange += inputChangeHandler;
        }

        public override void Disable()
        {
            base.Disable();

            if (ifKit != null)
            {
                ifKit.InputChange -= inputChangeHandler;
            }

        }
    }
}
