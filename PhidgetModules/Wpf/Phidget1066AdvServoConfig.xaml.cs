﻿using System;
using System.Collections.Generic;
using System.Linq;
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
using Phidgets;

namespace PhidgetModules.Wpf
{
	/// <summary>
	/// Interaction logic for Phidget1066AdvServoConfig.xaml
	/// </summary>
	public partial class Phidget1066AdvServoConfig : Window
	{
		public int Index { get; set; }

		public AdvancedServo AdvServo { get; set; }

		public ServoServo.ServoType ServoType { get; set; }

		public double Position { get; set; }

		public Phidget1066AdvServoConfig(AdvancedServo servo, int index, ServoServo.ServoType servoType, double position) {
			Index = index;
			AdvServo = servo;
			ServoType = servoType;
			Position = position;

			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			foreach (String servoType in Enum.GetNames(typeof(ServoServo.ServoType))) {
				//stop here

				if (servoType.Equals(ServoServo.ServoType.USER_DEFINED.ToString()))
					break;
				TypeComboBox.Items.Add(servoType);

				// Select the type we are given originally.
				if (servoType == ServoType.ToString()) {
					TypeComboBox.SelectedItem = servoType;
				}
			}

			PositionSlider.Value = Position;
		}

		private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {

			AdvServo.servos[Index].Type = (ServoServo.ServoType)Enum.Parse(typeof(ServoServo.ServoType), TypeComboBox.SelectedItem.ToString());
				
			PositionSlider.Maximum = AdvServo.servos[Index].PositionMax;
			PositionSlider.Minimum = AdvServo.servos[Index].PositionMin;

			PositionSlider.Value = Position;
		}

		private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			PositionText.Text = PositionSlider.Value.ToString("0.##");
		}

		private void Button_Save_Click(object sender, RoutedEventArgs e) {
			ServoType = (ServoServo.ServoType)Enum.Parse(typeof(ServoServo.ServoType), TypeComboBox.SelectedItem.ToString());
			Position = PositionSlider.Value;

			DialogResult = true;
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}

		

	}
}
