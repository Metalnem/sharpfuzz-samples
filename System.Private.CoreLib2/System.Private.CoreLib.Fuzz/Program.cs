﻿using System.Collections.Generic;
using System.Globalization;
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
				{ "DateTime.TryParse", DateTime_TryParse },
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

		private static void Convert_ToInt32(string path)
		{
			var text = File.ReadAllText(path);
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

		private static void DateTime_TryParse(string path)
		{
			var text = File.ReadAllText(path);

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

		private static void Decimal_Multiply(string path)
		{
			var text = File.ReadAllText(path);

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

		private static void Double_TryParse(string path)
		{
			var text = File.ReadAllText(path);

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

		private static void Guid_TryParse(string path)
		{
			var text = File.ReadAllText(path);
			Guid.TryParse(text, out _);
		}

		private static void IdnMapping_GetAscii(string path)
		{
			var text = File.ReadAllText(path);
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

		private static void TimeSpan_TryParse(string path)
		{
			var text = File.ReadAllText(path);

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

		private static void UnicodeEncoding_GetString(string path)
		{
			try
			{
				var bytes = File.ReadAllBytes(path);
				Encoding.Unicode.GetString(bytes);
			}
			catch (ArgumentException) { }
		}

		private static void UTF8Encoding_GetString(string path)
		{
			try
			{
				var bytes = File.ReadAllBytes(path);
				Encoding.UTF8.GetString(bytes);
			}
			catch (ArgumentException) { }
		}
	}
}
