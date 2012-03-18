using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Speech.Recognition;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DefaultModules.Events
{
	[DataContract]
	[MayhemModule("Speech Recognition", "Triggers when a phrase is heard")]
	public class SpeechRecognition : EventBase, IWpfConfigurable
	{
		[DataMember]
		private string phrase;

		private SpeechRecognitionEngine engine;

		protected override void OnEnabling(EnablingEventArgs e)
		{
			try
			{
				engine = new SpeechRecognitionEngine(CultureInfo.CurrentCulture);
			}
			catch
			{
				ErrorLog.AddError(ErrorType.Failure, "Unable to start up the Speech Recognition Engine");
				e.Cancel = true;
				return;
			}
			
			try
			{
				engine.SetInputToDefaultAudioDevice();
			}
			catch(InvalidOperationException)
			{
				ErrorLog.AddError(ErrorType.Failure, "No microphone is connected");
				e.Cancel = true;
				return;
			}

			engine.SpeechRecognized += RecognitionEngine_SpeechRecognized;

			if (!string.IsNullOrEmpty(phrase))
			{
				Grammar grammar = new Grammar(new GrammarBuilder(phrase));
				engine.LoadGrammar(grammar);
			}

			engine.RecognizeAsync(RecognizeMode.Multiple);
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			engine.RecognizeAsyncCancel();
		}

		protected override void OnDeleted()
		{
			engine.Dispose();
		}

		public WpfConfiguration ConfigurationControl
		{
			get { return new SpeechRecognitionConfig(phrase); }
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			var config = configurationControl as SpeechRecognitionConfig;
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
	}
}
