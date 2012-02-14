using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using KinectModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using Microsoft.Kinect;
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

		private KinectSensor sensor;

		private SpeechRecognitionEngine engine;

		private Stream stream;

		protected override void OnEnabling(EnablingEventArgs e)
		{
			sensor = (from sensorToCheck in KinectSensor.KinectSensors where sensorToCheck.Status == KinectStatus.Connected select sensorToCheck).FirstOrDefault();
			if (sensor == null)
			{
				ErrorLog.AddError(ErrorType.Failure, "Kinect not pluged in");
				e.Cancel = true;
				return;
			}

			sensor.Start();

			KinectAudioSource source = sensor.AudioSource;
			source.EchoCancellationMode = EchoCancellationMode.None;
			source.AutomaticGainControlEnabled = false;

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
			}
			catch
			{
				ErrorLog.AddError(ErrorType.Failure, "No Kinect found");
				e.Cancel = true;
				return;
			}

			var choices = new Choices();
			choices.Add(phrase);

			var gb = new GrammarBuilder { Culture = ri.Culture };
			gb.Append(choices);


			Grammar grammar = new Grammar(gb);
			
			engine.LoadGrammar(grammar);
			engine.SpeechRecognized += RecognitionEngine_SpeechRecognized;


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
			sensor.Stop();
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
			Logger.WriteLine("Said: " + e.Result.Text + ", with " + e.Result.Confidence + " confidence");
			if (e.Result.Confidence > .85)
			{
				Trigger();
			}
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
