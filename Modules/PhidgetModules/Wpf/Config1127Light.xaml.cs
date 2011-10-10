using System.Windows.Controls;
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

        public override string CheckValidity()
        {
            if (!(double.TryParse(textBoxTopValue.Text, out TopValue) && (TopValue >= 0 && TopValue <= 1000)))
            {
                return "Invalid Top Value";
            }
            return "";
        }

        public override void OnSave()
        {
            Increasing = (bool)IncreasingRadio.IsChecked;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            Validate();
        }

        public override string Title
        {
            get
            {
                return "Phidget - Light";
            }
        }
    }
}
