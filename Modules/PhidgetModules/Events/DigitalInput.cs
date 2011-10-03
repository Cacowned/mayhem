﻿using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Events
{
    [DataContract]
    //[MayhemModule("Phidget: Digital Input", "Triggers on a digital input")]
    public class DigitalInput : EventBase, IWpfConfigurable
    {
        // Which index do we want to be looking at?
        [DataMember]
        private int index;

        // Toggle when it goes on, or when it goes off?
        [DataMember]
        private bool onWhenOn;

        // The interface kit we are using for the sensors
        private InterfaceKit ifKit;

        private InputChangeEventHandler inputChangeHandler;

        protected override void OnLoadDefaults()
        {
            index = 0;
            onWhenOn = true;
        }

        protected override void OnAfterLoad()
        {
            this.ifKit = InterfaceFactory.Interface;
            inputChangeHandler = new InputChangeEventHandler(InputChanged);
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new PhidgetDigitalInputConfig(ifKit, index, onWhenOn); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            index = ((PhidgetDigitalInputConfig)configurationControl).Index;
            onWhenOn = ((PhidgetDigitalInputConfig)configurationControl).OnWhenOn;
        }

        public string GetConfigString()
        {
            string type = "turns on";
            if (!onWhenOn)
            {
                type = "turns off";
            }

            return String.Format("Triggers when input #{0} {1}", index, type);
        }

        // The input has changed, do the work here
        protected void InputChanged(object sender, InputChangeEventArgs ex)
        {
            // If e.CurrentValue is true, then it used to be false.
            // Trigger when appropriate

            // We are dealing with the right input
            if (ex.Index == index)
            {
                // If its true and we turn on when it turns on
                // then trigger
                if (ex.Value == true && onWhenOn)
                {
                    Trigger();
                }
                // otherwise, if it its off, and we trigger
                // when it turns off, then trigger
                else if (ex.Value == false && !onWhenOn)
                {
                    Trigger();
                }
            }
        }


        protected override void OnEnabling(EnablingEventArgs e)
        {
            ifKit.InputChange += inputChangeHandler;
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (ifKit != null)
            {
                ifKit.InputChange -= inputChangeHandler;
            }
        }
    }
}
