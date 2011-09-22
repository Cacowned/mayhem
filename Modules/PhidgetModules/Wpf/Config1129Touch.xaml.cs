using System;
using System.Windows;
using Phidgets;
using MayhemWpf.UserControls;
using Phidgets.Events;
using System.Windows.Threading;
using PhidgetModules.Wpf.UserControls;
using System.Windows.Controls;

namespace PhidgetModules.Wpf
{
    /// <summary>
    /// Interaction logic for Phidget1129TouchConfig.xaml
    /// </summary>
    public partial class Config1129Touch : PhidgetConfigControl
    {
        public bool OnTurnOn { get; private set; }

        public Config1129Touch(bool onTurnOn)
        {
            this.OnTurnOn = onTurnOn;
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
            OnWhenOn.IsChecked = OnTurnOn;
            OnWhenOff.IsChecked = !OnTurnOn;
        }

        public override void OnSave()
        {
            OnTurnOn = (bool)OnWhenOn.IsChecked;
        }
    }
}
