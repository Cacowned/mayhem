using System;
using System.Windows;
using Phidgets;
using MayhemWpf.UserControls;
using Phidgets.Events;

namespace PhidgetModules.Wpf
{
    /// <summary>
    /// Interaction logic for Phidget1129TouchConfig.xaml
    /// </summary>
    public partial class Phidget1129TouchConfig : IWpfConfiguration
    {
        public int Index;
        public bool OnTurnOn;

        private InterfaceKit ifKit;
        private Func<int, string> convertor;

        public Phidget1129TouchConfig(InterfaceKit ifKit, int index, bool onTurnOn, Func<int, string> conversion)
        {
            this.Index = index;
            this.ifKit = ifKit;
            this.OnTurnOn = onTurnOn;
            this.convertor = conversion;

            InitializeComponent();
        }

        public override string Title
        {
            get
            {
                return "Phidget - Touch";
            }
        }

        public override void OnLoad()
        {
            SensorDataBox.Index = Index;
            SensorDataBox.IfKit = ifKit;
            SensorDataBox.convertor = convertor;

            ifKit.Attach += ifKit_Attach;

            OnWhenOn.IsChecked = OnTurnOn;
            OnWhenOff.IsChecked = !OnTurnOn;
        }

        private void ifKit_Attach(object sender, AttachEventArgs e)
        {
            CanSave = true;
        }

        public override void OnSave()
        {
            Index = SensorDataBox.Index;
            OnTurnOn = (bool)OnWhenOn.IsChecked;
        }
    }
}
