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
using Phidgets.Events;
using System.Windows.Threading;

namespace PhidgetModules.Wpf.UserControls
{
    /// <summary>
    /// Interaction logic for SensorData.xaml
    /// </summary>
    public partial class SensorData : UserControl
    {
        public Func<int, double> convertor;

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(SensorData), new UIPropertyMetadata(0));

        public InterfaceKit IfKit
        {
            get { return (InterfaceKit)GetValue(IfKitProperty); }
            set { SetValue(IfKitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IfKit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IfKitProperty =
            DependencyProperty.Register("IfKit", typeof(InterfaceKit), typeof(SensorData), new UIPropertyMetadata(null));

        protected SensorChangeEventHandler handler;


        public SensorData()
        {
            InitializeComponent();

            handler = new SensorChangeEventHandler(SensorChange);

            /*
            for (int i = 0; i < IfKit.sensors.Count; i++)
            {
                SensorBox.Items.Add(i);
            }
            */
            //this.SensorBox.SelectedIndex = Index;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.IfKit.SensorChange += handler;
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.IfKit.SensorChange -= handler;
        }

        protected void SensorChange(object sender, SensorChangeEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                // We only care about the index we are looking at.
                if (e.Index == Index)
                {

                    if ((e.Value < 490) && (e.Value > 80))
                    {
                        this.ValueBox.Text = convertor(e.Value).ToString("0.##") + " cm";
                    }
                    else
                    {
                        this.ValueBox.Text = "Object Not Detected";
                    }

                }
            }));


        }

        private void SensorBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;

            Index = box.SelectedIndex;
        }

        

        
    }
}
