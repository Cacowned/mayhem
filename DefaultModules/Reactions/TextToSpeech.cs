using System;
using System.Runtime.Serialization;
using System.Windows;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using SpeechLib;
using System.Windows.Controls;

namespace DefaultModules.Reactions
{
    [DataContract]
    public class TextToSpeech : ReactionBase, IWpf
    {
        private string _message;
        [DataMember]
        private string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                SetConfigString();
            }
        }

        SpVoice voice;

        public TextToSpeech()
            : base("Text To Speech", "Speaks a given phrase.")
        {
            hasConfig = true;

            // Set the defaults
            Message = "Running Mayhem!";

            voice = new SpVoice();

        }

        public override void Perform()
        {
            voice.Speak(Message, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }

        /*
        public void WpfConfig()
        {
            var window = new TextToSpeechConfig(Message);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            window.ShowDialog();

            if (window.DialogResult == true) {
                Message = window.message;

                SetConfigString();
            }
        }
         * */

        public UserControl ConfigurationControl
        {
            get { return new TextToSpeechConfig(Message); }
        }

        public void OnSaved(UserControl configurationControl)
        {
            Message = ((TextToSpeechConfig)configurationControl).Message;
        }

        private void SetConfigString()
        {
            ConfigString = String.Format("Message: \"{0}\"", Message);
        }
    }
}
