using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Speech.Recognition;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace DefaultModules
{
	public static class SpeechRecognitionManager
	{
		private static SpeechRecognitionEngine engine;
		private static Dictionary<string, List<Action<SpeechRecognizedEventArgs>>> recognitions;

		static SpeechRecognitionManager()
		{
			recognitions = new Dictionary<string, List<Action<SpeechRecognizedEventArgs>>>();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void Recognize(string phrase, Action<SpeechRecognizedEventArgs> action)
		{
			// If this is the first phrase to listen for
			if (recognitions.Count == 0)
			{
				engine = new SpeechRecognitionEngine(CultureInfo.CurrentCulture);
				engine.SetInputToDefaultAudioDevice();
				engine.SpeechRecognized += SpeechRecognized;
			}

			// Add our phrase
			if (recognitions.ContainsKey(phrase))
			{
				// Add this action to the list
				recognitions[phrase].Add(action);
			}
			else
			{
				// Create a new list with only this action in it
				var recognizers = new List<Action<SpeechRecognizedEventArgs>>();
				recognizers.Add(action);

				recognitions.Add(phrase, recognizers);
			}

			ResetGrammar();
		}

		private static void ResetGrammar()
		{
			// stop recognizing
			engine.RecognizeAsyncCancel();

			engine.UnloadAllGrammars();

			// reset the engine grammar
			var choices = new Choices();
			foreach (string key in recognitions.Keys)
			{
				choices.Add(key);
			}


			var gb = new GrammarBuilder(choices);

			Grammar grammar = new Grammar(gb);

			engine.LoadGrammar(grammar);

			// start recognizing again
			engine.RecognizeAsync(RecognizeMode.Multiple);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void CancelRecognize(string phrase, Action<SpeechRecognizedEventArgs> action)
		{
			if (!recognitions.ContainsKey(phrase))
			{
				throw new InvalidOperationException("The given phrase has no registered handlers.");
			}

			var handlers = recognitions[phrase];

			if (!handlers.Contains(action))
			{
				throw new InvalidOperationException("The given handler is not registered with the given phrase.");
			}

			// Remove the handler
			handlers.Remove(action);

			// If we have no more handlers, remove the phrase
			if (handlers.Count == 0)
			{
				recognitions.Remove(phrase);
			}

			// We have nothing else to listen for
			if (recognitions.Count == 0)
			{
				engine.RecognizeAsyncCancel();
				engine.SpeechRecognized -= SpeechRecognized;
				engine = null;
				engine.Dispose();
			}
			else
			{
				ResetGrammar();
			}
		}

		private static void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
		{
			if (!recognitions.ContainsKey(e.Result.Text))
			{
				throw new InvalidOperationException("No Key exists with that spoken text");
			}

			var handlers = recognitions[e.Result.Text];

			Parallel.ForEach(handlers, action =>
			{
				if (action != null)
				{
					action(e);
				}
			});
		}
	}
}
