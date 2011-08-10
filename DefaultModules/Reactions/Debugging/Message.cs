using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Windows.Controls;
using MayhemDefaultStyles.UserControls;

namespace DefaultModules.Reactions.Debugging
{
    [DataContract]
    [MayhemModule("Debug: Message", "Generates debug output when triggered")]
    public class Message : ReactionBase, IWpfConfigurable
    {
        string message = "Debug Reaction was triggered";
        public Message()
        { }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public override void Perform()
        {
            Debug.WriteLine(String.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), message));
        }
        /*
        public void WpfConfig()
        {
            var window = new DebugMessageConfig(message);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            window.ShowDialog();

            if (window.DialogResult == true)
            {
                message = window.message;

                SetConfigString();
            }
        }
        */

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            message = ((DebugMessageConfig)configurationControl).Message;
            SetConfigString();
        }

        public override void SetConfigString()
        {
            ConfigString = message;
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { return new DebugMessageConfig(message); }
        }
    }
}
