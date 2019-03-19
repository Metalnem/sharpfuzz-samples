using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpFuzz;

namespace System.Private.CoreLib.Fuzz
{
	public class Program
	{
		private static readonly Dictionary<string, Action<string>> aflFuzz =
			new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
			{
				{ "Double.TryParse", Double_TryParse }
			};

		private static readonly Dictionary<string, ReadOnlySpanAction> libFuzzer =
		new Dictionary<string, ReadOnlySpanAction>(StringComparer.OrdinalIgnoreCase)
			{
				{ "Double.TryParse", Double_TryParse }
			};

		public static void Main(string[] args)
		{
			if (!(Environment.GetEnvironmentVariable("__LIBFUZZER_SHM_ID") is null))
			{
				Fuzzer.LibFuzzer.Run(libFuzzer[args[0]]);
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
				Fuzzer.Run(() => fuzzer(path));
			}
		}

		private static void Double_TryParse(string path)
		{
			var text = File.ReadAllText(path);

			if (Double.TryParse(text, out var d1))
			{
				var s = d1.ToString("R");
				var d2 = Double.Parse(s);

				if (d1 != d2)
				{
					throw new Exception();
				}
			}
		}

		private static void Double_TryParse(ReadOnlySpan<byte> data)
		{
			var text = Encoding.UTF8.GetString(data);

			if (Double.TryParse(text, out var d1))
			{
				var s = d1.ToString("R");
				var d2 = Double.Parse(s);

				if (d1 != d2)
				{
					throw new Exception();
				}
			}
		}
	}
}
