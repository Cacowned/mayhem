using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Speech.Recognition;
using System.IO;
using Microsoft.Speech.AudioFormat;
using System.Threading;

namespace KinectModules
{
	public static class SpeechRecognitionManager
	{
		private static Stream stream;
		private static SpeechRecognitionEngine engine;
		private static RecognizerInfo recognizer;
		
		// Pairs the phrase with a set of all the "event handlers" tied to it
		private static Dictionary<string, List<Action<SpeechRecognizedEventArgs>>> recognitions;

		static SpeechRecognitionManager()
		{
			recognitions = new Dictionary<string, List<Action<SpeechRecognizedEventArgs>>>();
		}

		public static void Recognize(string phrase, Action<SpeechRecognizedEventArgs> action)
		{
			// If this is the first phrase to listen for
			if (recognitions.Count == 0)
			{
				// Do some setup.
				stream = KinectAudioStreamManager.Get();

				recognizer = GetKinectRecognizer();

				// Something went wrong
				if (recognizer == null)
				{
					KinectAudioStreamManager.Release(ref stream);
					throw new InvalidOperationException("Unable to find a Kinect Speech Recognizer");
				}

				try
				{
					engine = new SpeechRecognitionEngine(recognizer.Id);
					engine.SpeechRecognized += SpeechRecognized;

					engine.SetInputToAudioStream(stream,
					new SpeechAudioFormatInfo(
						EncodingFormat.Pcm, 16000, 16, 1,
						32000, 2, null));

					//engine.RecognizeAsync();
				}
				catch (Exception e)
				{
					KinectAudioStreamManager.Release(ref stream);
					// rethrow
					throw e;
				}
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

			var gb = new GrammarBuilder { Culture = GetKinectRecognizer().Culture };
			gb.Append(choices);

			Grammar grammar = new Grammar(gb);

			engine.LoadGrammar(grammar);

			// start recognizing again
			engine.RecognizeAsync(RecognizeMode.Multiple);
		}

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
				KinectAudioStreamManager.Release(ref stream);
			}
			else
			{
				ResetGrammar();
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

		private static void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
		{
			if (!recognitions.ContainsKey(e.Result.Text))
			{
				throw new InvalidOperationException("No Key exists with that spoken text");
			}


			var handlers = recognitions[e.Result.Text];
			foreach (Action<SpeechRecognizedEventArgs> action in handlers)
			{
				ThreadPool.QueueUserWorkItem(o => action(e));
			}
		}
	}
}

/*
SpeechRecognitionManager.Recognize("foo", Recognize);

SpeechRecognitionManager.Recognize("bar", Recognize2);

SpeechRecognitionManager.CancelRecognize("foo", Recognize);
*/