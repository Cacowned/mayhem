using System;
using PhidgetModules.Wpf.UserControls;
using Phidgets;

namespace PhidgetModules.Wpf
{
    public partial class Config1103IrReflective : PhidgetConfigControl
    {
        public int Index;
        public bool OnTurnOn;

        protected Func<int, string> Convertor;
        public InterfaceKit IfKit;


        public Config1103IrReflective(bool onTurnOn)
        {
            this.OnTurnOn = onTurnOn;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            OnWhenOn.IsChecked = OnTurnOn;
            OnWhenOff.IsChecked = !OnTurnOn;
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
