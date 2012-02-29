using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Wpf
{
    public partial class Config1129Touch : PhidgetConfigControl
    {
        public bool OnTurnOn { get; private set; }

        public Config1129Touch(bool onTurnOn)
        {
            OnTurnOn = onTurnOn;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            OnWhenOn.IsChecked = OnTurnOn;
            OnWhenOff.IsChecked = !OnTurnOn;
        }

        public override string Title
        {
            get
            {
                return "Phidget: Touch";
            }
        }
        
        public override void OnSave()
        {
            OnTurnOn = (bool)OnWhenOn.IsChecked;
        }
    }
}
