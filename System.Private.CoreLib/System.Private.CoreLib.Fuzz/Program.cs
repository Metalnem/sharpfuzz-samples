using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SharpFuzz;

namespace System.Private.CoreLib.Fuzz
{
	public class Program
	{
		private static readonly Dictionary<string, Action<string>> fuzzers =
			new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
			{
				{ "DateTime.TryParse", DateTime_TryParse },
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

		public static void DateTime_TryParse(string path)
		{
			var text = File.ReadAllText(path);

			if (DateTime.TryParse(text, out var dt))
			{
				var s = dt.ToString("O");
				DateTime.Parse(s, null, DateTimeStyles.RoundtripKind);
			}
		}

		public static void TimeSpan_TryParse(string path)
		{
			var text = File.ReadAllText(path);
			TimeSpan.TryParse(text, out _);
		}
	}
}
