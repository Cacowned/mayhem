using System;
using System.Windows;
using Phidgets;
using MayhemWpf.UserControls;

namespace PhidgetModules.Wpf
{
    /// <summary>
    /// Interaction logic for _1133SoundConfig.xaml
    /// </summary>
    public partial class Phidget1133SoundConfig : IWpfConfiguration
    {
        public int Index;
        public double TopValue;
        public bool Increasing;

        public InterfaceKit ifKit;
        protected Func<int, string> convertor;

        public Phidget1133SoundConfig(InterfaceKit ifKit, int index, double topValue, bool increasing, Func<int, string> conversion)
        {
            this.Index = index;
            this.TopValue = topValue;

            this.ifKit = ifKit;
            this.convertor = conversion;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            SensorDataBox.Index = Index;
            SensorDataBox.IfKit = ifKit;
            SensorDataBox.convertor = convertor;

            textBoxTopValue.Text = TopValue.ToString();

            IncreasingRadio.IsChecked = Increasing;
            DecreasingRadio.IsChecked = !Increasing;
        }

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


        public override string Title
        {
            get
            {
                return "Phidget - Sound";
            }
        }
    }
}
