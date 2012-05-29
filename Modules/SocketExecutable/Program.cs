using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;

namespace SocketExecutable
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			string[] args = Environment.GetCommandLineArgs();

			if (args.Length > 1)
			{
				try
				{
					Write(args[1]);
				}
				catch (Exception)
				{
					// If we get in here, it means that mayhem wasn't open, but we aren't
					// showing any window anyways, so it doesn't really matter.
				}
			}



			/*
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
			*/
		}

		private static void Write(string message)
		{
			try
			{
				NamedPipeClientStream pipeClient =
					new NamedPipeClientStream(".", "mayhemSocketPipeName",
						PipeDirection.InOut, PipeOptions.None,
						TokenImpersonationLevel.Impersonation);

				pipeClient.Connect(200);
				StreamString ss = new StreamString(pipeClient);

				ss.WriteString(message);

				pipeClient.Close();
			}
			catch (Exception)
			{
				Console.WriteLine("ERROR: Mayhem with Socket Event is not running.");
			}
		}

		// Defines the data protocol for reading and writing strings on our stream
		public class StreamString
		{
			private Stream ioStream;
			private UnicodeEncoding streamEncoding;

			public StreamString(Stream ioStream)
			{
				this.ioStream = ioStream;
				streamEncoding = new UnicodeEncoding();
			}

			public int WriteString(string outString)
			{
				byte[] outBuffer = streamEncoding.GetBytes(outString);
				int len = outBuffer.Length;
				if (len > UInt16.MaxValue)
				{
					len = (int)UInt16.MaxValue;
				}

				ioStream.WriteByte((byte)(len / 256));
				ioStream.WriteByte((byte)(len & 255));
				ioStream.Write(outBuffer, 0, len);
				ioStream.Flush();

				return outBuffer.Length + 2;
			}
		}
	}
}
