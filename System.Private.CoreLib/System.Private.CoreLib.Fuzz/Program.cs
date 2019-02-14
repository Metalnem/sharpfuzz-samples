using System.Collections.Generic;
using System.IO;
using SharpFuzz;

namespace System.Private.CoreLib.Fuzz
{
	public class Program
	{
		private static readonly Dictionary<string, Action<string>> fuzzers =
			new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
			{
				{ "TimeSpan.TryParse", TimeSpan_TryParse },
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
				Fuzzer.Run(() => fuzzer(path));
			}
		}

		public static void TimeSpan_TryParse(string path)
		{
			var text = File.ReadAllText(path);
			TimeSpan.TryParse(text, out _);
		}
	}
}
