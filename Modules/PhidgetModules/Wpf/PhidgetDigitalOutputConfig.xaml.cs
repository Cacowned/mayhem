using System.Windows.Controls;
using System.Windows.Threading;
using MayhemWpf.UserControls;
using PhidgetModules.Reaction;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Wpf
{
    /// <summary>
    /// Interaction logic for PhidgetDigitalOutputConfig.xaml
    /// </summary>
    public partial class PhidgetDigitalOutputConfig : WpfConfiguration
    {
    	private InterfaceKit ifKit;

		public int Index { get; set; }

        public DigitalOutputType OutputType { get; set; }

        public PhidgetDigitalOutputConfig(int index, DigitalOutputType outputType)
        {
            Index = index;

            OutputType = outputType;

            InitializeComponent();
        }

        public override void OnLoad()
        {
        	ifKit = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: false);

            ifKit.Attach += ifKit_Attach;
			ifKit.Detach += ifKit_Detach;

            PopulateOutputs();

            OutputBox.SelectedIndex = Index;

            ControlBox.SelectedIndex = 0;

            switch (OutputType)
            {
                case DigitalOutputType.Toggle: ControlBox.SelectedIndex = 0;
                    break;
                case DigitalOutputType.On: ControlBox.SelectedIndex = 1;
                    break;
                case DigitalOutputType.Off: ControlBox.SelectedIndex = 2;
                    break;
            }
        }

		public override void OnClosing()
		{
			PhidgetManager.Release(ref ifKit);
		}

        private void ifKit_Attach(object sender, AttachEventArgs e)
        {
            PopulateOutputs();

			CheckCanSave();
        }

		private void ifKit_Detach(object sender, DetachEventArgs e)
		{
			CheckCanSave();
		}

		private void CheckCanSave()
		{
			if (ifKit.Attached)
			{
				// We need to have an error message
			}
		}

        private void PopulateOutputs()
        {
			if (ifKit.Attached)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
                {
                    CanSave = true;

					for (int i = 0; i < ifKit.outputs.Count; i++)
                    {
                        OutputBox.Items.Add(i);
                    }
                }));
            }
        }

        private void OutputBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            Index = box.SelectedIndex;
        }

        public override void OnSave()
        {
            Index = OutputBox.SelectedIndex;

            ComboBoxItem item = ControlBox.SelectedItem as ComboBoxItem;
            switch (item.Content.ToString())
            {
                case "Toggle": OutputType = DigitalOutputType.Toggle;
                    break;
                case "Turn On": OutputType = DigitalOutputType.On;
                    break;
                case "Turn Off": OutputType = DigitalOutputType.Off;
                    break;
            }
        }

        public override string Title
        {
            get
            {
                return "Phidget: Digital Output";
            }
        }
    }
}
