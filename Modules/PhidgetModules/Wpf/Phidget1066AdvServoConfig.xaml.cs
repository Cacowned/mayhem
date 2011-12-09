using System;
using System.Windows;
using System.Windows.Controls;
using MayhemCore;
using MayhemWpf.UserControls;
using Phidgets;

namespace PhidgetModules.Wpf
{
    /*
     * TODO: Make this configuration window more robust. For example, it should enable
     * and disable things when they servo controller is plugged in.
     */

    /// <summary>
    /// Interaction logic for Phidget1066AdvServoConfig.xaml
    /// </summary>
    public partial class Phidget1066AdvServoConfig : WpfConfiguration
    {
        public int Index { get; private set; }

        public AdvancedServo AdvServo { get; private set; }

        public ServoServo.ServoType ServoType { get; private set; }

        public double Position { get; private set; }

        public Phidget1066AdvServoConfig(AdvancedServo servo, int index, ServoServo.ServoType servoType, double position)
        {
            Index = index;
            AdvServo = servo;
            ServoType = servoType;
            Position = position;

            InitializeComponent();
        }

        public override string Title
        {
            get
            {
                return "Phidget - Adv. Servo";
            }
        }

        public override void OnLoad()
        {
            foreach (string servoType in Enum.GetNames(typeof(ServoServo.ServoType)))
            {
                //stop here

                if (servoType.Equals(ServoServo.ServoType.USER_DEFINED.ToString()))
                    break;

                TypeComboBox.Items.Add(servoType);

                // Select the type we are given originally.
                if (servoType == ServoType.ToString())
                {
                    TypeComboBox.SelectedItem = servoType;
                }
            }

            PositionSlider.Value = Position;
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AdvServo.servos.Count > 0)
            {
                try
                {
                    AdvServo.servos[Index].Type = (ServoServo.ServoType)Enum.Parse(typeof(ServoServo.ServoType), TypeComboBox.SelectedItem.ToString());
                }
                catch (Exception erf)
                {
                    Logger.WriteLine("Phidget1066AdvServoConfig: " + erf);
                }

                PositionSlider.Maximum = AdvServo.servos[Index].PositionMax;
                PositionSlider.Minimum = AdvServo.servos[Index].PositionMin;

                PositionSlider.Value = Position;
            }
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PositionText.Text = PositionSlider.Value.ToString("0.##");
        }

        public override void OnSave()
        {
            ServoType = (ServoServo.ServoType)Enum.Parse(typeof(ServoServo.ServoType), TypeComboBox.SelectedItem.ToString());
            Position = PositionSlider.Value;
        }
    }
}
