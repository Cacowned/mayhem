using System;
using System.Globalization;
using System.Runtime.Serialization;
using DebugModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DebugModules.Reactions
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

        public string GetConfigString()
        {
            return MessageText;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new DebugMessageConfig(MessageText); }
        }
    }
}
