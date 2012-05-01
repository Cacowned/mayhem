using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SerialManager
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
		 *  
		 * READ!!!!!
		 * While working through this, I came across a lot of the issues that people have
		 * been complaining about since .NET 2.0 regarding shutting down serial ports.
		 * I have tried tons of different solutions from these pages:
		 * http://connect.microsoft.com/VisualStudio/feedback/details/140018/serialport-crashes-after-disconnect-of-usb-com-port
		 * http://connect.microsoft.com/VisualStudio/feedback/details/426766/net-serialport-and-ftdi-usb-driver-unauthorizedaccessexception
		 * http://www.vbforums.com/showthread.php?t=558000
		 * http://social.msdn.microsoft.com/Forums/en-US/vblanguage/thread/7843cc2b-c460-40bd-882f-c562bff11a0a/
		 * 
		 * The best looking solution was found on this page:
		 * http://zachsaw.blogspot.com/2010/07/serialport-ioexception-workaround-in-c.html
		 * ^^ This page seems to be linked to in almost all of the above threads.
		 * Perhaps I wasn't doing it right, but I couldn't get any of these solutions to solve the problem.
		 * 
		 * 
		 * The solution that I finally was able to get to work is from here:
		 * http://social.msdn.microsoft.com/forums/en-US/Vsexpressvcs/thread/ce8ce1a3-64ed-4f26-b9ad-e2ff1d3be0a5/
		 * All I had to do was call the port.close method on a different thread.
		 * 
		 */

		// Pairs the port to a list of handlers for that data.
		private static Dictionary<SerialPort, List<Action<byte[], int>>> portHandlers;

		// Pairs the name of the port with the serial port object
		private static Dictionary<string, SerialPort> ports;

		// Pairs the name of the port with the settings used for that port
		private static Dictionary<string, SerialSettings> portSettings;

		private static object locker;

		#region Singleton
		// We are using a singleton here because it will have many methods
		// more than just get/release like in the other managers.
		private static SerialPortManager instance;
		public static SerialPortManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new SerialPortManager();
				}

				return instance;
			}
		}

		private SerialPortManager()
		{
			portHandlers = new Dictionary<SerialPort, List<Action<byte[], int>>>();
			ports = new Dictionary<string, SerialPort>();
			portSettings = new Dictionary<string, SerialSettings>();

			locker = new object();
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

		public static string[] AllPorts
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

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void ConnectPort(string portName, SerialSettings settings, Action<byte[], int> action = null)
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
					throw new InvalidOperationException(string.Format("The port {0} has already been opened with different settings.", portName));
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
				SerialPort port = new SerialPort(
					portName,
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

					port.DataReceived += this.DataReceived;
				}
				else
				{
					// Failure
					port.Close();
				}
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void ReleasePort(string portName, Action<byte[], int> action = null)
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
				// You have to close on a different thread, see note above.
				Thread closeDown = new Thread(() => this.Remove(portName));
				closeDown.Start();
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void Remove(string portName)
		{
			SerialPort port = ports[portName];

			// We have no more handlers for this port, remove it from everything
			port.DataReceived -= this.DataReceived;

			if (port.IsOpen)
			{
				port.Close();
			}

			portHandlers.Remove(port);
			portSettings.Remove(portName);
			ports.Remove(portName);
		}

		public void Write(string portName, string message)
		{
			lock(locker)
			{
				if (!ports.ContainsKey(portName))
				{
					throw new ArgumentException("The given port is not open", "portName");
				}

				SerialPort port = ports[portName];

				// See comments in the other write method.
				if (!port.IsOpen)
				{
					// This is needed to attempt to reattach if they unplug, then replug in
					// the com port.
					try
					{
						port.Open();
					}
					catch
					{
						// This will throw if they unplug the cable, then try to trigger.
					}
				}

				if (port.IsOpen)
				{
					port.Write(message);
				}
				Thread.Sleep(110);
			}
		}

		public void Write(string portName, byte[] buffer, int length)
		{
			lock (locker)
			{
				if (!ports.ContainsKey(portName))
				{
					throw new ArgumentException("The given port is not open", "portName");
				}

				SerialPort port = ports[portName];

				// We have to verify that the port is open here if we try to write
				if (!port.IsOpen)
				{
					// This is needed to attempt to reattach if they unplug, then replug in
					// the com port.
					try
					{
						port.Open();
					}
					catch
					{
						// This will throw if they unplug the cable, then try to trigger.
					}
				}

				if (port.IsOpen)
				{
					port.Write(buffer, 0, length);
				}
				Thread.Sleep(110);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			/*
			 * NOTE: We seem to recieve messages broken into two or more DataRecieved events.
			 * The fix: Wait 100 milliseconds, THEN read the data.
			 */
			Thread.Sleep(100);

			SerialPort port = sender as SerialPort;

			// In order to make sure that the different handlers don't use up
			// all the data, lets read it all here, and then pass a read only version around
			int numBytes = port.BytesToRead;
			byte[] input = new byte[numBytes];
			port.Read(input, 0, numBytes);

			List<Action<byte[], int>> handlers = portHandlers[port];

			/*
			 * NOTE: Okay, big frustration solved. We have to use the Parallel library instead
			 * of a foreach with QueueUserWorkItem inside of it because QueueUserWorkItem
			 * will reuse the same value from the foreach loop in every iteration, thus
			 * not actually doing the foreach properly.
			 * This is all discussed in this thread: http://stackoverflow.com/questions/616634/using-anonymous-delegates-with-net-threadpool-queueuserworkitem
			 */

			// Go through every data handler
			Parallel.ForEach(handlers, action =>
			{
				if (action != null)
				{
					action((byte[])input.Clone(), numBytes);
				}
			});

			/*
			DON't DO THIS:
			foreach (Action<byte[], int> action in handlers)
			{
				// Skip the null handlers.
				if (action != null)
				{
					ThreadPool.QueueUserWorkItem(o => action((byte[])input.Clone(), numBytes));
				}
			}*/
		}
	}
}