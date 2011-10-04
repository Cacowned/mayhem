using System.Windows.Controls;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Wpf
{

    public partial class Config1101IrDistance : PhidgetConfigControl
    {
        public double TopValue;
        public double BottomValue;

        public Config1101IrDistance(double topValue, double bottomValue)
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

        public override string Title
        {
            get
            {
                return "Phidget - IR Distance";
            }
        }

        public override string CheckValidity()
        {
            if (!double.TryParse(textBoxTopValue.Text, out TopValue) && TopValue >= 0)
            {
                return "You must enter a valid number for the top of the range";
            }
            else if (!double.TryParse(textBoxBottomValue.Text, out BottomValue) && TopValue >= 0)
            {
                return "You must enter a valid number for the bottom of the range";
            }
            else if (BottomValue > TopValue)
            {
                return "The bottom of the range must be lower than the top of the range";
            }

            return "";
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            Validate();
        }
    }
}
