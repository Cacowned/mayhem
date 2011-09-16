using System;
using System.Windows;
using Phidgets;
using MayhemWpf.UserControls;

namespace PhidgetModules.Wpf
{

    public partial class Phidget1101IRDistanceConfig : IWpfConfiguration
    {
        public int Index;
        public double TopValue;
        public double BottomValue;

        protected Func<int, string> convertor;
        private InterfaceKit IfKit;


        public Phidget1101IRDistanceConfig(InterfaceKit ifKit, int index, double topValue, double bottomValue, Func<int, string> conversion)
        {
            this.Index = index;
            this.TopValue = topValue;
            this.BottomValue = bottomValue;
            this.IfKit = ifKit;
            this.convertor = conversion;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            SensorDataBox.Index = Index;
            SensorDataBox.IfKit = IfKit;
            SensorDataBox.convertor = convertor;

            textBoxTopValue.Text = TopValue.ToString();
            textBoxBottomValue.Text = BottomValue.ToString();
        }

        public override bool OnSave()
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
                return true;
            }
            return false;
        }

        public override string Title
        {
            get
            {
                return "Phidget - IR Distance";
            }
        }
    }
}
