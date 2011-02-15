using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore.ModuleTypes;
using MayhemCore;
using SpeechLib;
using DefaultModules.Wpf;
using System.Windows;
using System.Runtime.Serialization;

namespace DefaultModules.Reactions
{
    [Serializable]
    public class TextToSpeech : ReactionBase, IWpf, ISerializable
    {
        protected string message = "Running Mayhem!";
        SpVoice voice;

        public TextToSpeech()
            : base("Text To Speech", "Speaks a given phrase.") {
            Setup();
        }

        public void Setup() {
            hasConfig = true;
            voice = new SpVoice();
            SetConfigString();
        }

        public override void Perform() {

            // If it's already running, then stop what it is saying.
            /*if (voice.Status.RunningState == SpeechRunState.SRSEIsSpeaking) {
                voice.Pause();
            }
            */
            voice.Speak(message, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);

        }

        public void WpfConfig() {
            var window = new TextToSpeechConfig(message);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            window.ShowDialog();

            if (window.DialogResult == true) {
                message = window.message;

                SetConfigString();
            }
        }

        private void SetConfigString() {
            ConfigString = String.Format("Message: \"{0}\"", message);
        }

        #region Serialization
        public TextToSpeech(SerializationInfo info, StreamingContext context) 
            : base (info, context)
        {
            
            message = info.GetString("Message");

            Setup();
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Message", message);
        }
        #endregion
    }
}
