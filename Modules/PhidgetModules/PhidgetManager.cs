using System;
using System.Collections.Generic;
using Phidgets;
using System.Diagnostics;

namespace PhidgetModules
{
	public static class PhidgetManager
	{
		private static Dictionary<Type, Phidget> openTypes;
		private static Dictionary<Type, int> counts;


		private static Stack<string> stack;

		private static List<string> opens;
		private static List<string> closes;

		// Flyweight the different Phidget types
		static PhidgetManager()
		{
			openTypes = new Dictionary<Type, Phidget>();
			counts = new Dictionary<Type, int>();

			opens = new List<string>();
			closes = new List<string>();

			stack = new Stack<string>();
		}

		public static T Get<T>(bool throwIfNotAttached = true) where T : Phidget, new()
		{
			// Get the Phidget type
			Type type = typeof(T);

			// Check if we have opened this type already
			if (!openTypes.ContainsKey(type))
			{
				// We haven't opened it
				T sensor = new T();
				// Try to open it to see if it is attached
				sensor.open();
				try
				{
					sensor.waitForAttachment(500);
				}
				catch (PhidgetException)
				{
					// No sensor is attached
					// Timeout was exceeded.
					if (throwIfNotAttached)
					{
						sensor.close();
						throw new InvalidOperationException("No Phidget with the type " + type.ToString() + " is attached");
					}
				}
				
				openTypes.Add(type, sensor);
			}

			if (!openTypes[type].Attached && throwIfNotAttached)
			{
				throw new InvalidOperationException("No Phidget with the type " + type.ToString() + " is attached");
			}

			StackTrace trace = new StackTrace();
			opens.Add(trace.GetFrame(1).GetMethod().DeclaringType.Name+": "+trace.GetFrame(1).GetMethod().Name);
			stack.Push(trace.GetFrame(1).GetMethod().DeclaringType.Name + ": " + trace.GetFrame(1).GetMethod().Name);

			IncrementCount(type);

			// Return the object
			return (T)openTypes[type];
		}

		private static void IncrementCount(Type type)
		 {
			// Increment the count in the dictionary
			if (!counts.ContainsKey(type))
			{
				counts.Add(type, 1);
			}
			else
			{
				counts[type]++;
			}

		}

		public static void Release<T>(ref T phidget) where T : Phidget
		{
			Type type = typeof(T);

			// We don't have anything with that type in the dictionary
			if (!openTypes.ContainsKey(type))
			{
				throw new InvalidOperationException("The specified Phidget hasn't been retrieved through the PhidgetManager");
			}
			else
			{
				// If we are already 0 or below, something is wrong.
				if (counts[type] <= 0)
				{
					throw new InvalidOperationException("Get must be called for the phidget device before Release");
				}

				StackTrace trace = new StackTrace();
				closes.Add(trace.GetFrame(1).GetMethod().DeclaringType.Name + ": " + trace.GetFrame(1).GetMethod().Name);
				stack.Pop();

				// Decrement the type
				counts[type]--;

				// If the type hits 0, there are no more references, clear it out.
				if (counts[type] == 0)
				{
					// no more connections, close it.
					openTypes[type].close();
					openTypes.Remove(type);
					counts.Remove(type);
				}
			}

			phidget = null;
		}

		public static int GetReferenceCount<T>() where T : Phidget
		{
			Type type = typeof(T);

			if (!counts.ContainsKey(type))
				return 0;
			else
				return counts[type];
		}
	}
}
