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
	[MayhemModule("Kinect: Speech Recognition", "Triggers when a phrase is heard")]
	public class KinectSpeech : EventBase, IWpfConfigurable
	{
		[DataMember]
		private string phrase;

		protected override void OnEnabling(EnablingEventArgs e)
		{
			try
			{
				SpeechRecognitionManager.Recognize(phrase, Recognized);
			}
			catch (InvalidOperationException ex)
			{
				ErrorLog.AddError(ErrorType.Failure, ex.Message);
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

		private void Recognized(SpeechRecognizedEventArgs e)
		{
			Logger.WriteLine("Said: " + e.Result.Text + ", with " + e.Result.Confidence + " confidence");
			if (e.Result.Confidence > .5)
			{
				Trigger();
			}
		}
	}
}
