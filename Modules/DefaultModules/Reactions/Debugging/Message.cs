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
    [MayhemModuleAttribute("Debug: Message", "Generates debug output when triggered")]
    public class Message : ReactionBase, IWpfConfigurable
    {
        private string MessageText { get; set; }

        public override void Perform()
        {
            Logger.WriteLine(String.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), MessageText));
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            MessageText = ((DebugMessageConfig)configurationControl).Message;
        }

        public override void SetConfigString()
        {
            ConfigString = MessageText;
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { return new DebugMessageConfig(MessageText); }
        }
    }
}
