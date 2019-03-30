using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SharpFuzz;

namespace CoreFX.Fuzz
{
	public class Program
	{
		private static readonly Dictionary<string, Action<Stream>> aflFuzz =
			new Dictionary<string, Action<Stream>>(StringComparer.OrdinalIgnoreCase)
			{
				{ "JsonDocument.Parse", JsonDocument_Parse }
			};

		private static readonly Dictionary<string, ReadOnlySpanAction> libFuzzer =
			new Dictionary<string, ReadOnlySpanAction>(StringComparer.OrdinalIgnoreCase)
			{
				{ "JsonDocument.Parse", JsonDocument_Parse }
			};

		public static void Main(string[] args)
		{
			if (!(Environment.GetEnvironmentVariable("__LIBFUZZER_SHM_ID") is null))
			{
				Fuzzer.LibFuzzer.Run(libFuzzer[args[0]]);
				return;
			}

			if (!(Environment.GetEnvironmentVariable("__AFL_SHM_ID") is null))
			{
				Fuzzer.OutOfProcess.Run(aflFuzz[args[0]]);
				return;
			}

			using (var stream = Console.OpenStandardInput())
			{
				var fuzzer = aflFuzz[args[0]];
				fuzzer(stream);
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
