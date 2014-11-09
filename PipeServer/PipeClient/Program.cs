using System;
using System.IO;
using System.IO.Pipes;

namespace PipeClient
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var client = new NamedPipeClientStream(".", "PIPE_NAME", PipeDirection.InOut))
			{
				client.Connect(2000);

				using (var pipe = new PipeStream(client))
				{
					Console.WriteLine("Message: " + pipe.Receive());

					pipe.Send("Hello!!!");
				}
			}

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
