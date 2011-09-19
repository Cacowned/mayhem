using System;
using System.Windows;
using Phidgets;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Wpf
{

    public partial class Config1101IRDistance : PhidgetConfigControl
    {
        public double TopValue;
        public double BottomValue;

        public Config1101IRDistance(double topValue, double bottomValue)
        {
            this.TopValue = topValue;
            this.BottomValue = bottomValue;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            textBoxTopValue.Text = TopValue.ToString();
            textBoxBottomValue.Text = BottomValue.ToString();
        }

        /*
        public override void OnSave()
        {
            if (!double.TryParse(textBoxTopValue.Text, out TopValue) && TopValue >= 0)
            {
                MessageBox.Show("You must enter a valid number for the top of the range");
            }
            else if (!double.TryParse(textBoxBottomValue.Text, out BottomValue) && TopValue >= 0)
            {
                MessageBox.Show("You must enter a valid number for the bottom of the range");
            }
            else if (BottomValue > TopValue)
            {
                MessageBox.Show("The bottom of the range must be lower than the top of the range");
            }
            else
            {
                Index = SensorDataBox.Index;
            }
        }
         */

        public override string Title
        {
            get
            {
                return "Phidget - IR Distance";
            }
        }
    }
}
