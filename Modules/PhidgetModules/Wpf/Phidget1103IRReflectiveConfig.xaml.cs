using System;
using System.Windows;
using Phidgets;
using MayhemWpf.UserControls;

namespace PhidgetModules.Wpf
{
    public partial class Phidget1103IRReflectiveConfig : IWpfConfiguration
    {
        public int Index;
        public bool OnTurnOn;

        protected Func<int, string> convertor;
        public InterfaceKit IfKit;


        public Phidget1103IRReflectiveConfig(InterfaceKit ifKit, int index, bool onTurnOn, Func<int, string> conversion)
        {
            this.Index = index;
            this.OnTurnOn = onTurnOn;

            this.convertor = conversion;
            this.IfKit = ifKit;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            SensorDataBox.Index = Index;
            SensorDataBox.IfKit = IfKit;
            SensorDataBox.convertor = convertor;

            OnWhenOn.IsChecked = OnTurnOn;
            OnWhenOff.IsChecked = !OnTurnOn;
        }

        public override bool OnSave()
        {
            OnTurnOn = (bool)OnWhenOn.IsChecked;
            Index = SensorDataBox.Index;

            return true;
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
