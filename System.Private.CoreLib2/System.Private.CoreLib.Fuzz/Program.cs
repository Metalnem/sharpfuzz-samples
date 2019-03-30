using System.Collections.Generic;
using System.Globalization;
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
				{ "Convert.ToInt32", Convert_ToInt32 },
				{ "DateTime.TryParse", DateTime_TryParse },
				{ "Decimal.Multiply", Decimal_Multiply },
				{ "Double.TryParse", Double_TryParse },
				{ "Guid.TryParse", Guid_TryParse },
				{ "IdnMapping.GetAscii", IdnMapping_GetAscii },
				{ "TimeSpan.TryParse", TimeSpan_TryParse },
				{ "UnicodeEncoding.GetString", UnicodeEncoding_GetString },
				{ "UTF8Encoding.GetString", UTF8Encoding_GetString }
			};

		private static readonly Dictionary<string, ReadOnlySpanAction> libFuzzer =
			new Dictionary<string, ReadOnlySpanAction>(StringComparer.OrdinalIgnoreCase)
			{
				{ "DateTime.TryParse", DateTime_TryParse }
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

		private static void Convert_ToInt32(Stream stream)
		{
			var bytes = ReadAllBytes(stream);
			var text = Encoding.UTF8.GetString(bytes);
			var fromBases = new int[] { 2, 8, 10, 16 };

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

		private static void DateTime_TryParse(Stream stream)
		{
			var bytes = ReadAllBytes(stream);
			var text = Encoding.UTF8.GetString(bytes);

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

		private static void Decimal_Multiply(Stream stream)
		{
			var bytes = ReadAllBytes(stream);
			var text = Encoding.UTF8.GetString(bytes);

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

		private static void Double_TryParse(Stream stream)
		{
			var bytes = ReadAllBytes(stream);
			var text = Encoding.UTF8.GetString(bytes);

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

		private static void Guid_TryParse(Stream stream)
		{
			var bytes = ReadAllBytes(stream);
			var text = Encoding.UTF8.GetString(bytes);

			Guid.TryParse(text, out _);
		}

		private static void IdnMapping_GetAscii(Stream stream)
		{
			var bytes = ReadAllBytes(stream);
			var text = Encoding.UTF8.GetString(bytes);
			var mapping = new IdnMapping();

			try
			{
				var ascii = mapping.GetAscii(text);
				var unicode = mapping.GetUnicode(ascii);
			}
			catch (ArgumentException) { }

			try
			{
				mapping.GetUnicode(text);
			}
			catch (ArgumentException) { }
		}

		private static void TimeSpan_TryParse(Stream stream)
		{
			var bytes = ReadAllBytes(stream);
			var text = Encoding.UTF8.GetString(bytes);

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
