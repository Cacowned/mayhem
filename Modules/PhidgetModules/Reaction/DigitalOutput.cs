using System;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using PhidgetModules.Wpf;
using Phidgets;
using MayhemWpf.UserControls;

namespace PhidgetModules.Reaction
{
    [DataContract]
    [MayhemModule("Phidget: Digital Output", "Triggers a digital output")]
    public class DigitalOutput : ReactionBase, IWpfConfigurable
    {
        #region Configuration
        // Which index do we want to be looking at?
        [DataMember]
        private int Index;

        [DataMember]
        private DigitalOutputType OutputType;
        #endregion

        // The interface kit we are using for the sensors
        private InterfaceKit ifKit;

        public DigitalOutput()
        {
            Index = 0;
            OutputType = DigitalOutputType.Toggle;
        }

        protected override void Initialize()
        {
            this.ifKit = InterfaceFactory.Interface;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new PhidgetDigitalOutputConfig(ifKit, Index, OutputType); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            Index = ((PhidgetDigitalOutputConfig)configurationControl).Index;
            OutputType = ((PhidgetDigitalOutputConfig)configurationControl).OutputType;
        }

        public string GetConfigString()
        {
            string type = "";

            switch (OutputType)
            {
                case DigitalOutputType.Toggle: type = "Toggle";
                    break;
                case DigitalOutputType.On: type = "Turn On";
                    break;
                case DigitalOutputType.Off: type = "Turn Off";
                    break;
            }

            return type + " output #" + Index;
        }

        public override void Perform()
        {
            switch (OutputType)
            {
                case DigitalOutputType.Toggle:
                    ifKit.outputs[Index] = !ifKit.outputs[Index];
                    break;
                case DigitalOutputType.On:
                    ifKit.outputs[Index] = true;
                    break;
                case DigitalOutputType.Off:
                    ifKit.outputs[Index] = false;
                    break;
            }
        }
    }

    // What action we should take on the digital output
    public enum DigitalOutputType
    {
        On,
        Off,
        Toggle
    }
}
