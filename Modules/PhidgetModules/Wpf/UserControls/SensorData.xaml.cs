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
        public Func<int, string> Convertor
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public InterfaceKit IfKit;

        public SensorData()
        {
            InitializeComponent();
        }

        public void Setup()
        {
            IfKit.Attach += IfKit_Attach;
            IfKit.SensorChange += SensorChange;

            SetUpSensorBox();
        }

        private void IfKit_Attach(object sender, AttachEventArgs e)
        {
            SetUpSensorBox();
        }

        private void SetUpSensorBox()
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                for (int i = 0; i < IfKit.sensors.Count; i++)
                {
                    SensorBox.Items.Add(i);
                }

                this.SensorBox.SelectedIndex = Index;

                if (IfKit.sensors.Count > 0)
                {
                    // We want to start with some data.
                    SetString(Convertor(IfKit.sensors[Index].Value));
                }
            }));
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
                    SetString(Convertor(e.Value));

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
