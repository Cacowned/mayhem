using System;
using System.Windows;
using Phidgets;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Wpf
{
    public partial class Config1133Sound : PhidgetConfigControl
    {
        public double TopValue;
        public bool Increasing;

        public Config1133Sound(double topValue, bool increasing)
        {
            this.TopValue = topValue;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            textBoxTopValue.Text = TopValue.ToString();

            IncreasingRadio.IsChecked = Increasing;
            DecreasingRadio.IsChecked = !Increasing;
        }

        public override string Title
        {
            get
            {
                return "Phidget - Sound";
            }
        }
    }
}
