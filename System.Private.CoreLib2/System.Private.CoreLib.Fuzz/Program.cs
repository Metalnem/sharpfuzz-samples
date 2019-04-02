using System.Globalization;
using System.IO;
using System.Text;
using SharpFuzz;

namespace System.Private.CoreLib.Fuzz
{
	public class Program
	{
		private static readonly byte[] buffer = new byte[1_000_000];
		private static readonly int[] fromBases = new int[] { 2, 8, 10, 16 };

		public static void Main(string[] args)
		{
			if (!(Environment.GetEnvironmentVariable("__LIBFUZZER_SHM_ID") is null))
			{
				switch (args[0])
				{
					case "DateTime.TryParse": Fuzzer.LibFuzzer.Run(DateTime_TryParse); return;
					default: throw new ArgumentException($"Unknown fuzzing function: {args[0]}");
				}
			}

			switch (args[0])
			{
				case "Convert.ToInt32": Fuzzer.Run(Convert_ToInt32); return;
				case "DateTime.TryParse": Fuzzer.Run(DateTime_TryParse); return;
				case "Decimal.Multiply": Fuzzer.Run(Decimal_Multiply); return;
				case "Double.TryParse": Fuzzer.Run(Double_TryParse); return;
				case "Guid.TryParse": Fuzzer.Run(Guid_TryParse); return;
				case "TimeSpan.TryParse": Fuzzer.Run(TimeSpan_TryParse); return;
				case "UnicodeEncoding.GetString": Fuzzer.Run(UnicodeEncoding_GetString); return;
				case "UTF8Encoding.GetString": Fuzzer.Run(UTF8Encoding_GetString); return;
				default: throw new ArgumentException($"Unknown fuzzing function: {args[0]}");
			}
		}

		private static void Convert_ToInt32(string text)
		{
			foreach (var fromBase in fromBases)
			{
				int? n1 = null;

				try
				{
					n1 = Convert.ToInt32(text, fromBase);
				}
				catch (ArgumentException) { }
				catch (FormatException) { }
				catch (OverflowException) { }

				if (n1.HasValue)
				{
					var s = Convert.ToString(n1.Value, fromBase);
					var n2 = Convert.ToInt32(s, fromBase);

					if (n1 != n2)
					{
						throw new Exception();
					}
				}
			}
		}

		private static void DateTime_TryParse(string text)
		{
			if (DateTime.TryParse(text, out var dt1))
			{
				var s = dt1.ToString("O");
				var dt2 = DateTime.Parse(s, null, DateTimeStyles.RoundtripKind);

				if (dt1 != dt2)
				{
					throw new Exception();
				}
			}
		}

		private static void DateTime_TryParse(ReadOnlySpan<byte> data)
		{
			var text = Encoding.UTF8.GetString(data);

			if (DateTime.TryParse(text, out var dt1))
			{
				var s = dt1.ToString("O");
				var dt2 = DateTime.Parse(s, null, DateTimeStyles.RoundtripKind);

				if (dt1 != dt2)
				{
					throw new Exception();
				}
			}
		}

		private static void Decimal_Multiply(string text)
		{
			if (Decimal.TryParse(text, out var d))
			{
				try
				{
					Decimal.Multiply(d, d);
				}
				catch (OverflowException) { }

				if (d != 0 && Decimal.Divide(d, d) != 1)
				{
					throw new Exception();
				}
			}
		}

		private static void Double_TryParse(string text)
		{
			if (Double.TryParse(text, out var d1))
			{
				var s = d1.ToString("G17");
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
				var s = d1.ToString("G17");
				var d2 = Double.Parse(s);

				if (d1 != d2)
				{
					throw new Exception();
				}
			}
		}

		private static void Guid_TryParse(string text)
		{
			Guid.TryParse(text, out _);
		}

		private static void TimeSpan_TryParse(string text)
		{
			if (TimeSpan.TryParse(text, out var t1))
			{
				var s = t1.ToString("c");
				var t2 = TimeSpan.Parse(s);

				if (t1 != t2)
				{
					throw new Exception();
				}
			}
		}

		private static void UnicodeEncoding_GetString(Stream stream)
		{
			var bytes = ReadAllBytes(stream);
			Encoding.Unicode.GetString(bytes);
		}

		private static void UTF8Encoding_GetString(Stream stream)
		{
			var bytes = ReadAllBytes(stream);
			Encoding.UTF8.GetString(bytes);
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
