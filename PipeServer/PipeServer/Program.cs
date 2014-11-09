using System;
using System.IO;
using System.IO.Pipes;

namespace PipeServer
{
	class Program
	{
		static void Main(string[] args)
		{
			var running = true;

			while (running) // loop only for 1 client
			{
				using (var server = new NamedPipeServerStream("PIPE_NAME", PipeDirection.InOut))
				{
					server.WaitForConnection();

					Console.WriteLine("Client connected");

					using (var pipe = new PipeStream(server))
					{
						pipe.Send("handshake");

						Console.WriteLine(pipe.Receive());

						server.WaitForPipeDrain();
						server.Flush();
					}
				}
			}

			Console.WriteLine("server closed");
			Console.Read();
		}
	}

	public class PipeStream : IDisposable
	{
		private readonly Stream _stream;
		private readonly Reader _reader;
		private readonly Writer _writer;

		public PipeStream(Stream stream)
		{
			_stream = stream;

			_reader = new Reader(_stream);
			_writer = new Writer(_stream);
		}

		public string Receive()
		{
			return _reader.ReadLine();
		}

		public void Send(string message)
		{
			_writer.WriteLine(message);
			_writer.Flush();
		}

		public void Dispose()
		{
			_reader.Dispose();
			_writer.Dispose();

			_stream.Dispose();
		}

		private class Reader : StreamReader
		{
			public Reader(Stream stream)
				: base(stream)
			{

			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(false);
			}
		}

		private class Writer : StreamWriter
		{
			public Writer(Stream stream)
				: base(stream)
			{

			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(false);
			}
		}
	}
}
