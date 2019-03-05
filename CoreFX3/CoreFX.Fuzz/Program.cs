using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SharpFuzz;

namespace CoreFX.Fuzz
{
	public class Program
	{
		private static readonly Dictionary<string, Action<string>> fuzzers =
			new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
			{
				{ "JsonDocument.Parse", JsonDocument_Parse }
			};

		public static void Main(string[] args)
		{
			var fuzzer = fuzzers[args[1]];
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
	}
}
