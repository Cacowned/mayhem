using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Wpf.UserControls
{
    /// <summary>
    /// Interaction logic for SensorData.xaml
    /// </summary>
    public partial class SensorData : UserControl
    {
        public Func<int, string> convertor;

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(SensorData), new UIPropertyMetadata(0));

        public InterfaceKit IfKit;

        public SensorData()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (IfKit != null)
            {
                this.IfKit.SensorChange += SensorChange;

                for (int i = 0; i < IfKit.sensors.Count; i++)
                {
                    SensorBox.Items.Add(i);
                }

                this.SensorBox.SelectedIndex = Index;

                // We want to start with some data.
                SetString(convertor(IfKit.sensors[Index].Value));
            }
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.IfKit.SensorChange -= SensorChange;
        }

        protected void SensorChange(object sender, SensorChangeEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
                   {
                       // We only care about the index we are looking at.
                       if (e.Index == Index)
                       {
                           SetString(convertor(e.Value));

                       }
                   }));

        }

        protected void SetString(string text)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
                   {
                       this.ValueBox.Text = text;
                   }));
        }

        private void SensorBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            Index = box.SelectedIndex;
        }
    }
}
