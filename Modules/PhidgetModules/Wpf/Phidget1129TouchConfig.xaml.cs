using System;
using System.Windows;
using Phidgets;
using MayhemWpf.UserControls;

namespace PhidgetModules.Wpf
{
    /// <summary>
    /// Interaction logic for Phidget1129TouchConfig.xaml
    /// </summary>
    public partial class Phidget1129TouchConfig : IWpfConfiguration
    {
        public int Index;
        public bool OnTurnOn;

        public InterfaceKit ifKit;
        protected Func<int, string> convertor;

        public Phidget1129TouchConfig(InterfaceKit ifKit, int index, bool onTurnOn, Func<int, string> conversion)
        {
            this.Index = index;
            this.ifKit = ifKit;
            this.OnTurnOn = onTurnOn;
            this.convertor = conversion;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            SensorDataBox.Index = Index;
            SensorDataBox.IfKit = ifKit;
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
                return "Phidget - Touch";
            }
        }
    }
}
