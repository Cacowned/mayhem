using System;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;
using MayhemCore.ModuleTypes;
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
        protected int Index
        {
            get;
            set;
        }

        [DataMember]
        protected DigitalOutputType OutputType
        {
            get;
            set;
        }
        #endregion

        // The interface kit we are using for the sensors
        protected InterfaceKit ifKit;

        protected override void Initialize()
        {
            base.Initialize();

            this.ifKit = InterfaceFactory.GetInterface();

            Index = 0;
            OutputType = DigitalOutputType.Toggle;
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { return new PhidgetDigitalOutputConfig(ifKit, Index, OutputType); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            Index = ((PhidgetDigitalOutputConfig)configurationControl).Index;
            OutputType = ((PhidgetDigitalOutputConfig)configurationControl).OutputType;
        }

        public override void SetConfigString()
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

            ConfigString = type + " output #" + Index;
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

            /*
            this.ifKit.outputs[index] = flag;
            flag = !flag;
             */
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
