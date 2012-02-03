using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using KinectModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using Microsoft.Research.Kinect.Audio;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace KinectModules.Events
{
    [DataContract]
    [MayhemModule("Kinect Speech Recognition", "Triggers when a phrase is heard")]
    public class KinectSpeech : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string phrase;

        private SpeechRecognitionEngine engine;

        private KinectAudioSource source;

        private Stream stream;

        protected override void OnEnabling(EnablingEventArgs e)
        {
			source = new KinectAudioSource();
			source.FeatureMode = true;
			source.AutomaticGainControl = false;
			source.SystemMode = SystemMode.OptibeamArrayOnly;

			RecognizerInfo ri = GetKinectRecognizer();

			if (ri == null)
			{
				ErrorLog.AddError(ErrorType.Failure, "Could not find Kinect speech recognizer");
				e.Cancel = true;
				return;
			}

			try
			{
				engine = new SpeechRecognitionEngine(ri.Id);
				engine.SpeechRecognized += RecognitionEngine_SpeechRecognized;
			}
			catch
			{
				ErrorLog.AddError(ErrorType.Failure, "No Kinect found");
				e.Cancel = true;
				return;
			}
			
			if (!string.IsNullOrEmpty(phrase))
			{
				Grammar grammar = new Grammar(new GrammarBuilder(phrase));
				engine.LoadGrammar(grammar);
			}

            stream = source.Start();
            engine.SetInputToAudioStream(stream,
                    new SpeechAudioFormatInfo(
                        EncodingFormat.Pcm, 16000, 16, 1,
                        32000, 2, null));

            engine.RecognizeAsync(RecognizeMode.Multiple);
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            engine.RecognizeAsyncCancel();
            source.Stop();
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new KinectSpeechConfig(phrase); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as KinectSpeechConfig;
            phrase = config.Phrase;
        }

        public string GetConfigString()
        {
            return phrase;
        }

        private void RecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //This first release of the Kinect language pack doesn't have a reliable confidence model, so 
            //we don't use e.Result.Confidence here.

            Logger.WriteLine("Said: " + e.Result.Text);
            Trigger();
        }

		private static RecognizerInfo GetKinectRecognizer()
		{
			Func<RecognizerInfo, bool> matchingFunc = r =>
			{
				string value;
				r.AdditionalInfo.TryGetValue("Kinect", out value);
				return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
			};
			return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
		}
    }
}
