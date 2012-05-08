using System;
using System.Windows;
using System.Windows.Controls;
using MayhemCore;
using MayhemWpf.UserControls;
using Phidgets;
using Phidgets.Events;
using System.Windows.Threading;

namespace PhidgetModules.Wpf
{
	public partial class Phidget1066AdvServoConfig : WpfConfiguration
    {
		private AdvancedServo servo;

        public int Index { get; private set; }

        public ServoServo.ServoType ServoType { get; private set; }

        public double Position { get; private set; }

        public Phidget1066AdvServoConfig(int index, ServoServo.ServoType servoType, double position)
        {
            Index = index;
            ServoType = servoType;
            Position = position;

            InitializeComponent();
        }

        public override string Title
        {
            get
            {
                return "Phidget: Adv. Servo";
            }
        }

        public override void OnLoad()
        {
			servo = PhidgetManager.Get<AdvancedServo>(false);
			servo.Attach += servo_Attach;
			servo.Detach += servo_Detach;

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

			// Set the text (this doesn't get called if position was 0)
        	PositionSlider_ValueChanged(null, null);

			SetAttached();
        }

		private void servo_Attach(object sender, AttachEventArgs e)
		{
			SetAttached();
		}

		private void servo_Detach(object sender, DetachEventArgs e)
		{
			SetAttached();
		}

		private void SetAttached()
		{
			Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
			{
				if (servo.Attached)
				{
					CanSave = true;
					Invalid.Visibility = Visibility.Collapsed;
				}
				else
				{
					CanSave = false;
					Invalid.Visibility = Visibility.Visible;
				}
			}));
		}


		public override void OnClosing()
		{
			servo.Attach -= servo_Attach;
            servo.Detach -= servo_Detach;

			PhidgetManager.Release<AdvancedServo>(ref servo);
		}

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (servo.servos.Count > 0)
            {
                try
                {
                    servo.servos[Index].Type = (ServoServo.ServoType)Enum.Parse(typeof(ServoServo.ServoType), TypeComboBox.SelectedItem.ToString());
                }
                catch (Exception erf)
                {
                    Logger.WriteLine("Phidget1066AdvServoConfig: " + erf);
                }

				PositionSlider.Maximum = servo.servos[Index].PositionMax;
				PositionSlider.Minimum = servo.servos[Index].PositionMin;

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
