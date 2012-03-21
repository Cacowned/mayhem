using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.IO;
using Microsoft.Speech.Recognition;

namespace KinectModules
{
	public static class KinectAudioStreamManager
	{
		// This is be retrieved through the KinectManager
		private static KinectSensor sensor;

		private static KinectAudioSource source;
		private static int count;

		public static Stream Get()
		{
			if (count == 0)
			{
				// This will throw if no Kinect is attached
				sensor = KinectManager.Get();
			}

			source = sensor.AudioSource;
			source.EchoCancellationMode = EchoCancellationMode.None;
			source.AutomaticGainControlEnabled = false;

			count++;

			Stream stream = source.Start();

			return stream;
		}

		public static void Release(ref Stream stream)
		{
			if (count <= 0)
			{
				throw new InvalidOperationException("Get must be called for the Kinect Audio Stream before the Release");
			}

			count--;
			stream = null;

			if (count == 0)
			{
				source.Stop();
				source = null;
			}
		}
	}
}
