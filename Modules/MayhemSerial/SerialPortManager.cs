using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Collections.ObjectModel;

namespace MayhemSerial
{
	public class SerialPortManager
	{
		/*
		 * TODO: It is very likely that this class needs to be modified to be
		 * thread safe.
		 * 
		 * TODO: Why do we need the int on the Action that tells them how long the array is?
		 * It makes sense in non managed languages, but for us, why don't we just pass the 
		 *  array and then the user can call .length on it?
		 */

		// Pairs the port to a list of handlers for that data.
		private static Dictionary<SerialPort, List<Action<byte[], int>>> portHandlers;

		// Pairs the name of the port with the serial port object
		private static Dictionary<string, SerialPort> ports;

		// Pairs the name of the port with the settings used for that port
		private static Dictionary<string, SerialSettings> portSettings;

		#region Singleton
		// We are using a singleton here because it will have many methods
		// more than just get/release like in the other managers.
		private static SerialPortManager _instance;
		public static SerialPortManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new SerialPortManager();
				}
				return _instance;
			}
		}

		private SerialPortManager()
		{
			portHandlers = new Dictionary<SerialPort, List<Action<byte[], int>>>();
			ports = new Dictionary<string, SerialPort>();
			portSettings = new Dictionary<string, SerialSettings>();
		}
		#endregion

		public string[] OpenPorts
		{
			get
			{
				string[] ports = portSettings.Keys.ToArray();
				Array.Sort(ports);
				return ports;
			}
		}

		public string[] AllPorts
		{
			get
			{
				// TODO: This just gets the data from the registry, so there could be
				// bad data. A possible fix would be in the old code in MayhemSerial.UpdatePortList.
				string[] ports = SerialPort.GetPortNames().Distinct().ToArray();
				Array.Sort(ports);
				return ports;
			}
		}

		public void ConnectPort(string portName, SerialSettings settings, Action<byte[], int> action)
		{
			// Do some parameter checking
			if (string.IsNullOrWhiteSpace(portName))
			{
				throw new ArgumentNullException(portName);
			}

			if (!AllPorts.Contains(portName))
			{
				throw new ArgumentException("The given port name doesn't exist", "portName");
			}

			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}

			// We should be able to work with the parameters given now.

			// First check that if we have opened this port, the settings are compatible
			if (portSettings.ContainsKey(portName))
			{
				if (portSettings[portName] != settings)
				{
					// Settings don't match
					throw new InvalidOperationException(string.Format("The port {0} has already been opened with different settings", portName));
				}
				else
				{
					// It's open and the settings match
					SerialPort port = ports[portName];

					// Add the handler
					portHandlers[port].Add(action);
				}
			}
			else
			{
				// We haven't seen this port before
				SerialPort port = new SerialPort(portName,
												settings.BaudRate,
												settings.Parity,
												settings.DataBits,
												settings.StopBits);

				// TODO: This could very well throw an exception, but we want to only
				// deal with the ones we have to, so we will deal with them when they come up
				port.Open();

				if (port.IsOpen)
				{
					// Success?
					ports.Add(portName, port);
					portSettings.Add(portName, settings);
					List<Action<byte[], int>> handlers = new List<Action<byte[], int>>();
					handlers.Add(action);
					portHandlers.Add(port, handlers);

					port.DataReceived += DataReceived;
					// TODO: Do we need to handle the Disposed event?

				}
				else
				{
					// Failure
					port.Close();
				}
			}
		}

		public void ReleasePort(string portName, Action<byte[], int> action)
		{
			// Do some parameter checking
			if (string.IsNullOrWhiteSpace(portName))
			{
				throw new ArgumentNullException(portName);
			}

			if (!portSettings.Keys.Contains(portName))
			{
				throw new ArgumentException("The given port name hasn't been opened", "portName");
			}

			SerialPort port = ports[portName];

			if (!portHandlers[port].Contains(action))
			{
				throw new ArgumentException("The given action isn't registered with the port", "action");
			}

			portHandlers[port].Remove(action);

			if (portHandlers[port].Count == 0)
			{
				// We have no more handlers for this port, remove it from everything
				port.Close();
				portHandlers.Remove(port);
				portSettings.Remove(portName);
				ports.Remove(portName);
			}
		}

		public void Write(string portName, string message)
		{
			if (!ports.ContainsKey(portName))
			{
				throw new ArgumentException("The given port is not open", "portName");
			}

			SerialPort port = ports[portName];

			port.Write(message);
		}

		public void Write(string portName, byte[] buffer, int length)
		{
			if (!ports.ContainsKey(portName))
			{
				throw new ArgumentException("The given port is not open", "portName");
			}

			SerialPort port = ports[portName];

			port.Write(buffer, 0, length);
		}

		private void DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			SerialPort port = sender as SerialPort;

			// In order to make sure that the different handlers don't use up
			// all the data, lets read it all here, and then pass a read only version around

			int numBytes = port.BytesToRead;
			byte[] input = new byte[numBytes];
			port.Read(input, 0, numBytes);

			// Go through every data handler
			foreach (Action<byte[], int> action in portHandlers[port])
			{
				ThreadPool.QueueUserWorkItem(o => action((byte[])input.Clone(), numBytes));
			}
		}

	}
}