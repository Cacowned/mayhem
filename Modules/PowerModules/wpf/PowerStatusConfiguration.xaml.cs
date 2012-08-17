using System;
using System.Windows;
using System.Windows.Forms;
using MayhemWpf.UserControls;

namespace PowerModules
{
    /// <summary>
    /// Interaction logic for PowerStatusConfiguration.xaml
    /// </summary>
    public partial class PowerStatusConfiguration : WpfConfiguration
    {
        public PowerStatusChoice ChosenStatus { get; private set; }
        public int Percentage { get; private set; }
        public BatteryChargeStatus ChosenBCS { get; private set; }

        public PowerStatusConfiguration(PowerStatusChoice chosenstatus, BatteryChargeStatus chosenBCS, int percentage)
        {
            ChosenStatus = chosenstatus;
            Percentage = percentage;
            ChosenBCS = chosenBCS;
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Power Status"; }
        }

        public override void OnLoad()
        {
            CanSave = true;

            switch (ChosenStatus)
            {
                case PowerStatusChoice.Percentage:
                    RadioPercent.IsChecked = true;
                    break;
                default:
                    RadioBCS.IsChecked = true;
                    break;
            }

            switch (ChosenBCS)
            {
                case BatteryChargeStatus.Charging:
                    BatteryChargeSelectionList.SelectedIndex = 0;
                    break;
                case BatteryChargeStatus.Low:
                    BatteryChargeSelectionList.SelectedIndex = 1;
                    break;
                default:
                    BatteryChargeSelectionList.SelectedIndex = 2;
                    break;
            }

            BatteryPercentageBox.Text = Percentage.ToString();
        }

        public override void OnSave()
        {
            if (RadioBCS.IsChecked == true)
                ChosenStatus = PowerStatusChoice.PowerState;
            else
                ChosenStatus = PowerStatusChoice.Percentage;

            switch (BatteryChargeSelectionList.SelectedIndex)
            {
                case 0:
                    ChosenBCS = BatteryChargeStatus.Charging;
                    break;
                case 1:
                    ChosenBCS = BatteryChargeStatus.Low;
                    break;
                case 2:
                    ChosenBCS = BatteryChargeStatus.Critical;
                    break;
            }

            Percentage = Int32.Parse(BatteryPercentageBox.Text);
        }

        private void TextChanged(object sender, RoutedEventArgs e)
        {
            if (RadioBCS.IsChecked == true)
            {
                BatteryChargeSelectionList.IsEnabled = true;
                BatteryPercentageBox.IsEnabled = false;
            }
            else
            {
                BatteryChargeSelectionList.IsEnabled = false;
                BatteryPercentageBox.IsEnabled = true;
            }

            textInvalid.Text = CheckValidity();
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private string CheckValidity()
        {
            string s = "Invalid";
            int percent;
            bool badpercent = !(Int32.TryParse(BatteryPercentageBox.Text, out percent) && (percent >= 3 && percent <= 98));

            if (badpercent && RadioPercent.IsChecked != true)
            {
                BatteryPercentageBox.Text = "30";
                badpercent = false;
            }

            if (badpercent)
                s += " percentage";

            CanSave = !badpercent;
            return CanSave ? string.Empty : s;
        }
    }
}
