﻿using System;
using System.Runtime.Serialization;
using System.Speech.Recognition;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using DefaultModules.Wpf;
using System.Globalization;

namespace DefaultModules.Events
{
	[DataContract]
	[MayhemModule("Speech Recognition", "Triggers when a phrase is heard")]
	public class SpeechRecognition : EventBase, IWpfConfigurable
	{
		[DataMember]
		private string phrase;

		private SpeechRecognitionEngine engine;

		protected override void OnAfterLoad()
		{
			engine = new SpeechRecognitionEngine(new CultureInfo("en-US"));
			engine.SetInputToDefaultAudioDevice();
			engine.SpeechRecognized += RecognitionEngine_SpeechRecognized;

			if (!string.IsNullOrEmpty(phrase))
			{
				Grammar grammar = new Grammar(new GrammarBuilder(phrase));
				engine.LoadGrammar(grammar);
			}
		}

		protected override void OnEnabling(EnablingEventArgs e)
		{
			engine.RecognizeAsync(RecognizeMode.Multiple);
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			engine.RecognizeAsyncCancel();
		}

		public WpfConfiguration ConfigurationControl
		{
			get { return new SpeechRecognitionConfig(phrase); }
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			var config = configurationControl as SpeechRecognitionConfig;
			phrase = config.Phrase;

			Grammar grammar = new Grammar(new GrammarBuilder(phrase));
			engine.UnloadAllGrammars();
			engine.LoadGrammar(grammar);
		}

		public string GetConfigString()
		{
			return phrase;
		}

		private void RecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
		{
			Logger.WriteLine("Said: "+e.Result.Text+", with "+e.Result.Confidence+" confidence");
			if (e.Result.Confidence > .85)
			{
				Trigger();
			}
		}
	}
}
