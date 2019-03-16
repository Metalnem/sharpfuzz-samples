using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SharpFuzz;

namespace CoreFX.Fuzz
{
	public class Program
	{
		private static readonly Dictionary<string, Action<string>> aflFuzz =
			new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
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
				Fuzzer.LibFuzzer.Run(libFuzzer["JsonDocument.Parse"]);
				return;
			}

			var fuzzer = aflFuzz[args[1]];
			var path = args[0];

			if (Environment.GetEnvironmentVariable("__AFL_SHM_ID") is null)
			{
				fuzzer(path);
			}
			else
			{
				Fuzzer.OutOfProcess.Run(() => fuzzer(path));
			}
		}

		private static void JsonDocument_Parse(string path)
		{
			using (var file = File.OpenRead(path))
			{
				try
				{
					JsonDocument.Parse(file);
				}
				catch (JsonReaderException) { }
			}
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
