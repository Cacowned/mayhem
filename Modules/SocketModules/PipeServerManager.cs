using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using MayhemCore;

namespace SocketModules
{
	public static class PipeServerManager
	{
		// Volatile is used as hint to the compiler that this data
		// member will be accessed by multiple threads.
		private volatile static bool _shouldStop;

		private static int maxThreads = 4;

		private static PipeServer[] threads;
		private static Thread[] servers;

		private static Dictionary<string, List<Action>> handlers;

		private static Thread serverWatcher;

		static PipeServerManager()
		{
			threads = new PipeServer[maxThreads];
			servers = new Thread[maxThreads];

			handlers = new Dictionary<string, List<Action>>();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void Listen(string phrase, Action action)
		{
			bool shouldStart = false;

			// If we don't originally have anything then we are stopped
			if (handlers.Count == 0)
			{
				shouldStart = true;
			}

			if (!handlers.ContainsKey(phrase))
			{
				handlers[phrase] = new List<Action>();
				handlers[phrase].Add(action);
			}

			if (shouldStart)
			{
				// start up the thread
				SpinUp();
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void Forget(string phrase, Action action)
		{
			if (!handlers.ContainsKey(phrase))
			{
				throw new InvalidOperationException(phrase + " is not currently being listened for.");
			}

			handlers[phrase].Remove(action);

			if (handlers[phrase].Count == 0)
			{
				handlers.Remove(phrase);
			}

			if (handlers.Count == 0)
			{
				// spin down the thread
				SpinDown();
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void SpinUp()
		{
			serverWatcher = new Thread(DoWork);
			serverWatcher.Start();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void SpinDown()
		{
			_shouldStop = true;
			serverWatcher.Join();
		}

		private static void DoWork()
		{
			// reset our flag
			_shouldStop = false;

			// While we aren't being told to stop
			while (!_shouldStop)
			{
				for (int j = 0; j < maxThreads; j++)
				{
					if (servers[j] != null)
					{
						if (servers[j].Join(250))
						{
							string message = threads[j].Message;
							if (message != null && handlers.ContainsKey(message))
							{
								var actions = handlers[message];

								Parallel.ForEach(actions, action =>
								{
									action();
								});
							}

							Logger.WriteLine(string.Format("Server thread[{0}] finished.", servers[j].ManagedThreadId));
							servers[j] = null;
						}
					}
					else
					{
						// it's null, start it up
						threads[j] = new PipeServer();
						servers[j] = new Thread(threads[j].DoWork);
						servers[j].Start();
						Logger.WriteLine(string.Format("Starting up thread [{0}]", servers[j].ManagedThreadId));
					}
				}
				Thread.Sleep(250);
			}

			// End all of the server instances by creating "clients" for them.
			// TODO: This is a true hack, we should be more graceful in how we
			// terminate the threads.
			Parallel.ForEach(servers, server =>
			{
				NamedPipeClientStream pipeClient =
				new NamedPipeClientStream(".", "mayhemSocketPipeName",
					PipeDirection.Out, PipeOptions.None,
					TokenImpersonationLevel.Impersonation);

				try
				{
					pipeClient.Connect(250);
				}
				finally
				{
					pipeClient.Close();
					pipeClient.Dispose();
				}
			});

			Logger.WriteLine("worker thread: terminating gracefully.");
		}
	}
}
