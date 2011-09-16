using System;
using System.Globalization;
using System.Runtime.Serialization;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using SpeechLib;

namespace DefaultModules.Reactions
{
    [DataContract]
    [MayhemModule("Text To Speech", "Speaks a given phrase")]
    public class TextToSpeech : ReactionBase, IWpfConfigurable
    {
        #region Configuration Properties
        [DataMember]
        private string Message
        {
            get;
            set;
        }
        #endregion

        SpVoice voice;

        protected override void Initialize()
        {
            base.Initialize();

            voice = new SpVoice();
        }

        public override void Perform()
        {
            voice.Speak(Message, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { return new TextToSpeechConfig(Message); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            Message = ((TextToSpeechConfig)configurationControl).Message;
        }

        public override void SetConfigString()
        {
            ConfigString = String.Format(CultureInfo.CurrentCulture, EnglishStrings.TextToSpeech_ConfigString, Message);
        }
    }
}
