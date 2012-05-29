using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;

namespace SocketApp
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine("Interactive Mode!");
				Console.WriteLine("Type a phrase and hit enter to send. Type 'quit' to close.");

				string message = "";
				while (true)
				{
					message = Console.ReadLine();

					if (message == "quit")
					{
						break;
					}
					else
					{
						Write(message);
					}
				}
			}
			else
			{
				Write(args[0]);
			}
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
