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


		protected override void OnEnabling(EnablingEventArgs e)
		{
			try
			{
				SpeechRecognitionManager.Recognize(phrase, Recognized);
			}
			catch
			{
				ErrorLog.AddError(ErrorType.Failure, "Unable to start up the Speech Recognition Engine");
				e.Cancel = true;
				return;
			}
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			SpeechRecognitionManager.CancelRecognize(phrase, Recognized);
		}

		public WpfConfiguration ConfigurationControl
		{
			get
			{
				return new SpeechRecognitionConfig(phrase);
			}
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

		private void Recognized(SpeechRecognizedEventArgs e)
		{
			Logger.WriteLine("Said: " + e.Result.Text + ", with " + e.Result.Confidence + " confidence");
			if (e.Result.Confidence > .85)
			{
				Trigger();
			}
		}
	}
}
