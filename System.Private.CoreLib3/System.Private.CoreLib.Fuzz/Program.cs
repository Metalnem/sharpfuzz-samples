using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpFuzz;

namespace System.Private.CoreLib.Fuzz
{
	public class Program
	{
		private static readonly byte[] buffer = new byte[1_000_000];

		private static readonly Dictionary<string, Action<Stream>> aflFuzz =
			new Dictionary<string, Action<Stream>>(StringComparer.OrdinalIgnoreCase)
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

			if (!(Environment.GetEnvironmentVariable("__AFL_SHM_ID") is null))
			{
				Fuzzer.Run(aflFuzz[args[0]]);
				return;
			}

			using (var stream = Console.OpenStandardInput())
			{
				var fuzzer = aflFuzz[args[0]];
				fuzzer(stream);
			}
		}

		private static void Double_TryParse(Stream stream)
		{
			var bytes = ReadAllBytes(stream);
			var text = Encoding.UTF8.GetString(bytes);

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

		private static byte[] ReadAllBytes(Stream stream)
		{
			int size = stream.Read(buffer, 0, buffer.Length);

			if (size == buffer.Length)
			{
				throw new Exception();
			}

			return buffer.AsSpan(0, size).ToArray();
		}
	}
}
