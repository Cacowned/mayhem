using System;
using System.Collections.Generic;
using System.Linq;
using MayhemWpf.UserControls;
using System.Windows.Forms;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PreGsocTest1
{
    /// <summary>
    /// Interaction logic for PowerStatusConfiguration.xaml
    /// </summary>
    public partial class PowerStatusConfiguration : WpfConfiguration
    {
        public PowerStatusChoice ChosenStatus { get; private set; }
        public float Percentage { get; private set; }
        public BatteryChargeStatus ChosenBCS { get; private set; }
        public int RemainingTime { get; private set; } //seconds
        public PowerStatusConfiguration(PowerStatusChoice chosenstatus, float percentage, BatteryChargeStatus chosenBCS, int remainingTime)
        {
            ChosenStatus = chosenstatus;
            Percentage = percentage;
            ChosenBCS = chosenBCS;
            RemainingTime = remainingTime;
            InitializeComponent();
        }
        public override string Title
        {
            get { return "Power Status"; }
        }
        public override void OnLoad()
        {
            CanSave = true;
            //shouldCheckVailidity = true;
            switch (ChosenStatus)
            {
                case PowerStatusChoice.Percentage:
                    RadioPercent.IsChecked = true;
                    break;
                case PowerStatusChoice.RemainingTime:
                    RadioLifeTime.IsChecked = true;
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
            BatteryTimeRemainingBox.Text = RemainingTime.ToString();
            BatteryPercentageBox.Text = Percentage.ToString();
        }
        public override void OnSave()
        {
            if (RadioBCS.IsChecked == true)
                ChosenStatus = PowerStatusChoice.PowerState;
            else if (RadioLifeTime.IsChecked == true)
                ChosenStatus = PowerStatusChoice.RemainingTime;
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
            Percentage = float.Parse(BatteryPercentageBox.Text);
            RemainingTime = Int32.Parse(BatteryTimeRemainingBox.Text);
        }
        private void TextChanged(object sender, RoutedEventArgs e)
        {
            if (RadioBCS.IsChecked == true)
            {
                BatteryChargeSelectionList.IsEnabled = true;
                BatteryPercentageBox.IsEnabled = false;
                BatteryTimeRemainingBox.IsEnabled = false;
            }
            else if (RadioLifeTime.IsChecked == true)
            {
                BatteryChargeSelectionList.IsEnabled = false;
                BatteryPercentageBox.IsEnabled = false;
                BatteryTimeRemainingBox.IsEnabled = true;
            }
            else
            {
                BatteryChargeSelectionList.IsEnabled = false;
                BatteryPercentageBox.IsEnabled = true;
                BatteryTimeRemainingBox.IsEnabled = false;
            }
            textInvalid.Text = CheckValidity();
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private string CheckValidity()
        {
            string s = "Invalid";
            float percent;
            int minutes;
            bool badtime = !(Int32.TryParse(BatteryTimeRemainingBox.Text, out minutes) && (minutes >= 2 && minutes <= 600));
            bool badpercent = !(float.TryParse(BatteryPercentageBox.Text, out percent) && (percent >= 3.0 && percent <= 98.0));
            if (badtime && RadioLifeTime.IsChecked != true)
            {
                BatteryTimeRemainingBox.Text = "30";
                badtime = false;
            }
            if (badpercent && RadioPercent.IsChecked != true)
            {
                BatteryPercentageBox.Text = "30";
                badpercent = false;
            }
            if (badpercent)
                s += " percentage";
            if (badtime)
                s += " time";
            CanSave = !(badpercent || badtime);
            return CanSave ? string.Empty : s;
        }
    }
}
