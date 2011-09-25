using System;
using System.Globalization;
using System.Runtime.Serialization;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;

namespace DefaultModules.Reactions
{
    [DataContract]
    [MayhemModule("Debug: Message", "Generates debug output when triggered")]
    public class Message : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string MessageText { get; set; }

        public override void Perform()
        {
            Logger.WriteLine(String.Format(CultureInfo.CurrentCulture, "{0}: {1}", DateTime.Now.ToLongTimeString(), MessageText));
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            MessageText = ((DebugMessageConfig)configurationControl).Message;
        }

        public override void SetConfigString()
        {
            ConfigString = MessageText;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new DebugMessageConfig(MessageText); }
        }
    }
}
