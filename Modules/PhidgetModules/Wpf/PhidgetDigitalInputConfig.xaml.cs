using System.Windows.Threading;
using MayhemWpf.UserControls;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Wpf
{
    /// <summary>
    /// Interaction logic for PhidgetDigitalInputConfig.xaml
    /// </summary>
    public partial class PhidgetDigitalInputConfig : WpfConfiguration
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
            IfKit.Attach += ifKit_Attach;

            GoesOnRadio.IsChecked = OnWhenOn;
            TurnsOffRadio.IsChecked = !OnWhenOn;
        }

        private void ifKit_Attach(object sender, AttachEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                CanSave = true;

                for (int i = 0; i < IfKit.inputs.Count; i++)
                {
                    InputBox.Items.Add(i);
                }

                this.InputBox.SelectedIndex = Index;
            }));
        }

        public override void OnSave()
        {
            Index = InputBox.SelectedIndex;
            OnWhenOn = (bool)GoesOnRadio.IsChecked;
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
