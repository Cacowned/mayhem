using System.IO;
using System.IO.Pipes;
using System.Threading;
using MayhemCore;
using SocketModules.LowLevel;

namespace SocketModules
{
	public class PipeServer
	{
		private int maxThreads = 4;

		public string Message { get; private set; }

		// This method will be called when the thread is started.
		public void DoWork()
		{
			NamedPipeServerStream pipeServer =
					new NamedPipeServerStream("mayhemSocketPipeName", PipeDirection.InOut, maxThreads);

			int threadId = Thread.CurrentThread.ManagedThreadId;

			// Wait for a client to connect
			//pipeServer.BeginWaitForConnection()
			pipeServer.WaitForConnection();

			Logger.WriteLine(string.Format("Client connected on thread[{0}].", threadId));

			try
			{
				// Read the request from the client. Once the client has
				// written to the pipe its security token will be available.
				StreamString ss = new StreamString(pipeServer);

				Message = ss.ReadString();
			}
			// Catch the IOException that is raised if the pipe is broken
			// or disconnected.
			catch (IOException e)
			{
				Logger.WriteLine(string.Format("ERROR: {0}", e.Message));
			}

			pipeServer.Close();
		}
	}
}
