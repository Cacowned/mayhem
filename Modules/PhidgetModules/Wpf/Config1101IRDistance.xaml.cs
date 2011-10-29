using System.Windows.Controls;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Wpf
{
    public partial class Config1101IrDistance : PhidgetConfigControl
    {
        public double TopValue
        {
            get;
            private set;
        }

        public double BottomValue
        {
            get;
            private set;
        }

        public Config1101IrDistance(double topValue, double bottomValue)
        {
            TopValue = topValue;
            BottomValue = bottomValue;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            textBoxTopValue.Text = TopValue.ToString();
            textBoxBottomValue.Text = BottomValue.ToString();
        }

        public override string Title
        {
            get
            {
                return "Phidget - IR Distance";
            }
        }

        public override string CheckValidity()
        {
            double topValue;
            double bottomValue;

            if (!double.TryParse(textBoxTopValue.Text, out topValue) && TopValue >= 0)
            {
                return "You must enter a valid number for the top of the range";
            }

            if (!double.TryParse(textBoxBottomValue.Text, out bottomValue) && TopValue >= 0)
            {
                return "You must enter a valid number for the bottom of the range";
            }

            if (BottomValue > TopValue)
            {
                return "The bottom of the range must be lower than the top of the range";
            }

            TopValue = topValue;
            BottomValue = bottomValue;

            return string.Empty;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            Validate();
        }
    }
}
