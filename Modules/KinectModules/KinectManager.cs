using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectModules
{
	public static class KinectManager
	{
		private static KinectSensor sensor;
		private static int count;

		static KinectManager()
		{
			count = 0;
		}

		public static KinectSensor Get()
		{
			if (sensor == null)
			{
				// Select the (a?) Kinect that is plugged in
				sensor = (from sensorToCheck in KinectSensor.KinectSensors
						  where sensorToCheck.Status == KinectStatus.Connected
						  select sensorToCheck)
						  .FirstOrDefault();

				// If there is nothing plugged in
				if (sensor == null)
				{
					throw new InvalidOperationException("No Kinect Attached");
				}

				// Let's start the sensor
				sensor.Start();
			}
			else
			{
				// We've already gotten the sensor, but are trying to get it again
				if (sensor.Status != KinectStatus.Connected)
				{
					// It's not attached
					throw new InvalidOperationException("No Kinect Attached");
				}
			}

			count++;
			return sensor;
		}

		public static void Release(ref KinectSensor sensor)
		{
			// If we are already 0 or below, something is wrong.
			if (count <= 0)
			{
				throw new InvalidOperationException("Get must be called for the Kinect device before Release");
			}

			count--;
			// remove their reference
			sensor = null;

			// remove our reference?
			if (count == 0)
			{
				KinectManager.sensor.Stop();
				KinectManager.sensor = null;
			}
		}
	}
}
