using System;
using System.IO;
using Sample;
using SharpFuzz;

namespace Google.Protobuf.Fuzz
{
	public class Program
	{
		private static readonly MessageParser[] parsers = new MessageParser[]
		{
			M0.Parser,
			M1.Parser,
			M2.Parser,
			M3.Parser,
			M4.Parser,
			M5.Parser,
			M6.Parser,
			M7.Parser,
			M8.Parser,
			M9.Parser,
			M10.Parser,
			M11.Parser,
			M12.Parser,
			M13.Parser,
			M14.Parser,
			M15.Parser,
			M16.Parser,
			M17.Parser,
			M18.Parser,
			M19.Parser,
			M20.Parser,
			M21.Parser,
			M22.Parser,
			M23.Parser,
			M24.Parser,
			M25.Parser
		};

		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(stream =>
			{
				using (var memory = new MemoryStream())
				{
					stream.CopyTo(memory);

					foreach (var parser in parsers)
					{
						try
						{
							memory.Seek(0, SeekOrigin.Begin);
							parser.ParseFrom(memory);
						}
						catch { }
					}
				}
			});
		}
	}
}
