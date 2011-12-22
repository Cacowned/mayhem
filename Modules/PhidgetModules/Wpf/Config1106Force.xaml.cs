using System.Windows.Controls;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Wpf
{
    public partial class Config1106Force : PhidgetConfigControl
    {
        public double TopValue
        {
            get;
            private set;
        }

        public bool Increasing
        {
            get;
            private set;
        }

        public Config1106Force(double topValue, bool increasing)
        {
            TopValue = topValue;
            Increasing = increasing;

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
                return "Phidget - Force";
            }
        }

        public override string CheckValidity()
        {
            double topValue;

            if (!(double.TryParse(textBoxTopValue.Text, out topValue) && (topValue >= 0 && topValue <= 1000)))
            {
                return "Invalid Top Value";
            }

            TopValue = topValue;

            return string.Empty;
        }

        public override void OnSave()
        {
            Increasing = (bool)IncreasingRadio.IsChecked;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            Validate();
        }
    }
}
