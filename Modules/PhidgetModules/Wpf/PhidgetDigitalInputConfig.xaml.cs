﻿using System.Windows.Controls;
using Phidgets;
using System.Windows;
using MayhemDefaultStyles.UserControls;

namespace PhidgetModules.Wpf
{
    /// <summary>
    /// Interaction logic for PhidgetDigitalInputConfig.xaml
    /// </summary>
    public partial class PhidgetDigitalInputConfig : IWpfConfiguration
    {
        public int Index { get; set; }

        public InterfaceKit IfKit { get; set; }

        // If true, then trigger when the digital input
        // turns on, otherwise when the digital input
        // turns off
        public bool OnWhenOn { get; set; }

        public PhidgetDigitalInputConfig(InterfaceKit ifKit, int index, bool onWhenOn)
        {
            this.Index = index;
            this.IfKit = ifKit;
            this.OnWhenOn = onWhenOn;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            for (int i = 0; i < IfKit.inputs.Count; i++)
            {
                InputBox.Items.Add(i);
            }

            this.InputBox.SelectedIndex = Index;

            GoesOnRadio.IsChecked = OnWhenOn;
            TurnsOffRadio.IsChecked = !OnWhenOn;
        }

        public override bool OnSave()
        {
            Index = InputBox.SelectedIndex;

            OnWhenOn = (bool)GoesOnRadio.IsChecked;
            return true;
        }

        public override string Title
        {
            get
            {
                return "Phidget - Digital Input";
            }
        }
    }
}
