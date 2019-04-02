using System;
using System.IO;
using System.Text.Json;
using SharpFuzz;

namespace CoreFX.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			if (!(Environment.GetEnvironmentVariable("__LIBFUZZER_SHM_ID") is null))
			{
				switch (args[0])
				{
					case "JsonDocument.Parse": Fuzzer.LibFuzzer.Run(JsonDocument_Parse); return;
					default: throw new ArgumentException($"Unknown fuzzing function: args[0]");
				}
			}

			switch (args[0])
			{
				case "JsonDocument.Parse": Fuzzer.OutOfProcess.Run(JsonDocument_Parse); return;
				default: throw new ArgumentException($"Unknown fuzzing function: args[0]");
			}
		}

		private static void JsonDocument_Parse(Stream stream)
		{
			try
			{
				JsonDocument.Parse(stream);
			}
			catch (JsonReaderException) { }
		}

		private static void JsonDocument_Parse(ReadOnlySpan<byte> data)
		{
			try
			{
				JsonDocument.Parse(data.ToArray());
			}
			catch (JsonReaderException) { }
		}
	}
}
