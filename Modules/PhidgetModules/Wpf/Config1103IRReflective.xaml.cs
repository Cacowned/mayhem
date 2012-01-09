﻿using System;
using PhidgetModules.Wpf.UserControls;
using Phidgets;

namespace PhidgetModules.Wpf
{
    public partial class Config1103IrReflective : PhidgetConfigControl
    {
        public bool OnTurnOn
        {
            get;
            private set;
        }

        protected Func<int, string> Convertor;
        public InterfaceKit IfKit;

        public Config1103IrReflective(bool onTurnOn)
        {
            OnTurnOn = onTurnOn;

            InitializeComponent();
        }

        public override void OnLoad()
        {
			// The reason this is backwards is because of how the IR Reflective sensor says it detects things.
			// Read the comment in the event class.
            OnWhenOn.IsChecked = !OnTurnOn;
            OnWhenOff.IsChecked = OnTurnOn;
        }

		public override void OnSave()
		{
			OnTurnOn = (bool)OnWhenOn.IsChecked;
		}

        public override string Title
        {
            get
            {
                return "Phidget - Proxmiity";
            }
        }
    }
}
