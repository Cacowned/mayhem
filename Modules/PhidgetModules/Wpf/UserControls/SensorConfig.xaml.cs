using System;
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
using MayhemWpf.UserControls;
using Phidgets;
using System.Windows.Threading;
using Phidgets.Events;

namespace PhidgetModules.Wpf.UserControls
{
    /// <summary>
    /// Interaction logic for SensorConfig.xaml
    /// </summary>
    public partial class SensorConfig : IWpfConfiguration
    {
        public int Index { get; private set; }
        public InterfaceKit IfKit { get; private set; }
        public Func<int, string> Convertor { get; private set; }

        public PhidgetConfigControl Sensor { get; private set; }

        public SensorConfig(InterfaceKit ifKit, int index, Func<int, string> conversion, PhidgetConfigControl control)
        {
            this.Index = index;
            this.IfKit = ifKit;
            this.Convertor = conversion;

            this.Sensor = control;

            InitializeComponent();
        }

        public override string Title
        {
            get
            {
                return Sensor.Title;
            }
        }

        public override void OnLoad()
        {
            SensorDataBox.Index = Index;
            SensorDataBox.IfKit = IfKit;
            SensorDataBox.convertor = Convertor;

            sensorControl.Content = Sensor;

            IfKit.Attach += ifKit_Attach;

            this.CanSavedChanged += PhidgetConfig_CanSavedChanged;

            // If we have detected sensors already, then enable the save button
            if (IfKit.sensors.Count > 0)
                CanSave = true;
        }

        public override void OnSave()
        {
            Index = SensorDataBox.Index;
        }

        public override void OnClosing()
        {
            IfKit.Attach -= ifKit_Attach;
            this.CanSavedChanged -= PhidgetConfig_CanSavedChanged;
        }

        private void PhidgetConfig_CanSavedChanged(bool canSave)
        {
            System.Windows.Visibility visible = System.Windows.Visibility.Visible;
            if (canSave)
                visible = System.Windows.Visibility.Collapsed;

            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                this.textInvalid.Visibility = visible;
            }));
        }

        private void ifKit_Attach(object sender, AttachEventArgs e)
        {
            CanSave = true;
        }
    }
}
