﻿using System;
using System.Windows;
using Phidgets;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Wpf
{
    /// <summary>
    /// Interaction logic for _1133SoundConfig.xaml
    /// </summary>
    public partial class Config1133Sound : PhidgetConfigControl
    {
        public double TopValue;
        public bool Increasing;

        public Config1133Sound(double topValue, bool increasing)
        {
            this.TopValue = topValue;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            textBoxTopValue.Text = TopValue.ToString();

            IncreasingRadio.IsChecked = Increasing;
            DecreasingRadio.IsChecked = !Increasing;
        }

        /*
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
        */

        public override string Title
        {
            get
            {
                return "Phidget - Sound";
            }
        }
    }
}
