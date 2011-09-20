using System;
using System.Windows;
using Phidgets;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Wpf
{
    public partial class Config1103IRReflective : PhidgetConfigControl
    {
        public int Index;
        public bool OnTurnOn;

        protected Func<int, string> convertor;
        public InterfaceKit IfKit;


        public Config1103IRReflective(bool onTurnOn)
        {
            this.OnTurnOn = onTurnOn;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            OnWhenOn.IsChecked = OnTurnOn;
            OnWhenOff.IsChecked = !OnTurnOn;
        }

        /*
        public override void OnSave()
        {
            OnTurnOn = (bool)OnWhenOn.IsChecked;
        }
         */

        public override string Title
        {
            get
            {
                return "Phidget - Proxmiity";
            }
        }
    }
}
