using PhidgetModules.Wpf.UserControls;
using Phidgets.Events;

namespace PhidgetModules.Wpf
{
	public partial class ConfigDigitalInput : PhidgetConfigControl
    {
    	// If true, then trigger when the digital input
        // turns on, otherwise when the digital input
        // turns off
        public bool OnWhenOn { get; set; }

        public ConfigDigitalInput(bool onWhenOn)
        {
            OnWhenOn = onWhenOn;

            InitializeComponent();
        }

        public override void OnLoad()
        {
        	GoesOnRadio.IsChecked = OnWhenOn;
            TurnsOffRadio.IsChecked = !OnWhenOn;
        }

		private void InputChange(object sender, InputChangeEventArgs e)
		{

		}

        public override void OnSave()
        {
            OnWhenOn = (bool)GoesOnRadio.IsChecked;
        }

        public override string Title
        {
            get
            {
                return "Phidget: Digital Input";
            }
        }
    }
}
