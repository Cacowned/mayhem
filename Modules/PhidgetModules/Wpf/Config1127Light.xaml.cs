using System;
using System.Windows;
using Phidgets;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Wpf
{
    public partial class Config1127Light : PhidgetConfigControl
    {
        public double TopValue;
        public bool Increasing;

        public Config1127Light(double topValue, bool increasing)
        {
            this.TopValue = topValue;
            this.Increasing = increasing;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            textBoxTopValue.Text = TopValue.ToString();

            IncreasingRadio.IsChecked = Increasing;
            DecreasingRadio.IsChecked = !Increasing;
        }

        /*
        public override void OnSave()
        {
            if (!double.TryParse(textBoxTopValue.Text, out TopValue) && TopValue >= 0)
            {
                MessageBox.Show("You must enter a valid number");
            }
            else
            {
                Increasing = (bool)IncreasingRadio.IsChecked;
                Index = SensorDataBox.Index;
            }
        }
        */

        public override string Title
        {
            get
            {
                return "Phidget - Light";
            }
        }
    }
}
