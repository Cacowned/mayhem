using System.Globalization;
using System.Runtime.Serialization;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using SpeechLib;

namespace DefaultModules.Reactions
{
    [DataContract]
    [MayhemModule("Text To Speech", "Speaks a given phrase")]
    public class TextToSpeech : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string Message
        {
            get;
            set;
        }

        private SpVoice voice;

        protected override void OnAfterLoad()
        {
            voice = new SpVoice();
        }

        public override void Perform()
        {
            voice.Speak(Message, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new TextToSpeechConfig(Message); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            Message = ((TextToSpeechConfig)configurationControl).Message;
        }

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.TextToSpeech_ConfigString, Message);
        }
    }
}
