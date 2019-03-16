using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using SharpFuzz;

namespace CoreFX.Fuzz
{
	public class Program
	{
		private static readonly char[] buffer = new char[8192];
		private static readonly string[] formats = new string[] { "C", "D", "G", "N", "R" };

		private static readonly FieldInfo squareThresholdField;
		private static readonly FieldInfo reducerThresholdField;

		private static readonly int[] squareThresholds = new int[] { 4, 32, Int32.MaxValue };
		private static readonly int[] reducerThresholds = new int[] { 0, 32, Int32.MaxValue };

		private const string headerString =
			"AAEAAAD/////AQAAAAAAAAAMAgAAAEJDb3JlRlguRnV6eiwgVmVyc2lvbj" +
			"0xLjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51" +
			"bGwFAQAAABdDb3JlRlguRnV6ei5Qcm9ncmFtK0JpbgMAAAABQQFCAUM=";

		private static readonly byte[] headerBytes = Convert.FromBase64String(headerString);

		private static readonly Dictionary<string, Action<string>> aflFuzz =
			new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
			{
				{ "BigInteger.DivRem", BigInteger_DivRem },
				{ "BigInteger.ModPow", BigInteger_ModPow },
				{ "BigInteger.TryParse", BigInteger_TryParse },
				{ "BinaryFormatter.Deserialize", BinaryFormatter_Deserialize },
				{ "DataContractJsonSerializer.ReadObject", DataContractJsonSerializer_ReadObject },
				{ "DataContractSerializer.ReadObject", DataContractSerializer_ReadObject },
				{ "HttpUtility.UrlEncode", HttpUtility_UrlEncode },
				{ "PEReader.GetMetadataReader", PEReader_GetMetadataReader },
				{ "Regex.Match", Regex_Match },
				{ "Utf8Parser.TryParseDateTime", Utf8Parser_TryParseDateTime },
				{ "Utf8Parser.TryParseDouble", Utf8Parser_TryParseDouble },
				{ "Utf8Parser.TryParseTimeSpan", Utf8Parser_TryParseTimeSpan },
				{ "XmlReader.Create", XmlReader_Create },
				{ "XmlSerializer.Deserialize", XmlSerializer_Deserialize },
				{ "ZipArchive.Entries", ZipArchive_Entries }
			};

		private static readonly Dictionary<string, ReadOnlySpanAction> libFuzzer =
			new Dictionary<string, ReadOnlySpanAction>(StringComparer.OrdinalIgnoreCase)
			{
				{ "XmlSerializer.Deserialize", XmlSerializer_Deserialize }
			};

		private static readonly Lazy<List<Regex>> regexes = new Lazy<List<Regex>>(() => new List<Regex>
		{
			new Regex("([(][(](?<t>[^)]+)[)])?(?<a>[^[]+)[[](?<ia>.+)[]][)]?", RegexOptions.None),
			new Regex("[(][(](?<cast>[^)]+)[)](?<arg>[^)]+)[)]", RegexOptions.None),
			new Regex("^([a-zA-Z]{1,8})(-[a-zA-Z0-9]{1,8})*$", RegexOptions.None),
			new Regex("(?<a>[^[]+)[[](?<ia>.+)[]]", RegexOptions.None),
			new Regex("CN=(.*[^,]+).*", RegexOptions.None)
		});

		[DataContract]
		public class Obj
		{
			[DataMember] public int A = 0;
			[DataMember] public double B = 0;
			[DataMember] public DateTime C = DateTime.MinValue;
			[DataMember] public bool D = false;
			[DataMember] public List<int> E = null;
			[DataMember] public string[] F = null;
		}

		[Serializable]
		public class Bin
		{
			[DataMember] public int A = 0;
			[DataMember] public double B = 0;
			[DataMember] public string[] C = null;
		}

		static Program()
		{
			var bigIntegerCalculator = typeof(BigInteger).Assembly.GetType("System.Numerics.BigIntegerCalculator");
			squareThresholdField = bigIntegerCalculator.GetField("SquareThreshold", BindingFlags.NonPublic | BindingFlags.Static);
			reducerThresholdField = bigIntegerCalculator.GetField("ReducerThreshold", BindingFlags.NonPublic | BindingFlags.Static);
		}

		public static void Main(string[] args)
		{
			if (!(Environment.GetEnvironmentVariable("__LIBFUZZER_SHM_ID") is null))
			{
				Fuzzer.LibFuzzer.Run(libFuzzer["XmlSerializer.Deserialize"]);
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

		private static void BigInteger_DivRem(string path)
		{
			var bytes = File.ReadAllBytes(path);

			if (bytes.Length == 0 || bytes.Length > 8192)
			{
				return;
			}

			var span = bytes.AsSpan(1);
			var len = span.Length;

			var l1 = ((bytes[0] & 0x3f) * len) / 0x3f;
			var l2 = len - l1;

			var s1 = bytes[0] & 0x40;
			var s2 = bytes[0] & 0x80;

			var a = new BigInteger(span.Slice(0, l1).ToArray());
			var b = new BigInteger(span.Slice(l1).ToArray());

			if (b.IsZero)
			{
				return;
			}

			if (s1 == 0 && a >= 0)
			{
				a = BigInteger.Negate(a);
			}

			if (s2 == 0 && b >= 0)
			{
				b = BigInteger.Negate(b);
			}

			var d = BigInteger.DivRem(a, b, out var r);

			if (a.IsZero && !(d.IsZero && r.IsZero))
			{
				throw new Exception();
			}

			if (b * d + r != a)
			{
				throw new Exception();
			}
		}

		private static void BigInteger_ModPow(string path)
		{
			var bytes = File.ReadAllBytes(path);

			if (bytes.Length <= 2 || bytes.Length > 8192)
			{
				return;
			}

			var span = bytes.AsSpan(3);
			var len = span.Length;

			var l1 = (bytes[0] * len) / 255;
			var l2 = (bytes[1] * (len - l1)) / 255;
			var l3 = len - l1 - l2;

			var sa = bytes[2] & 1;
			var sc = bytes[2] & 2;

			var a = new BigInteger(span.Slice(0, l1).ToArray());
			var b = new BigInteger(span.Slice(l1, l2).ToArray());
			var c = new BigInteger(span.Slice(l1 + l2).ToArray());

			if (c.IsZero)
			{
				return;
			}

			if (sa == 0 && a >= 0)
			{
				a = BigInteger.Negate(a);
			}

			if (b < 0)
			{
				b = BigInteger.Negate(b);
			}

			if (sc == 0 && c >= 0)
			{
				c = BigInteger.Negate(c);
			}

			BigInteger? result = null;

			foreach (var squareThreshold in squareThresholds)
			{
				foreach (var reducerThreshold in reducerThresholds)
				{
					squareThresholdField.SetValue(null, squareThreshold);
					reducerThresholdField.SetValue(null, reducerThreshold);

					if (result is null)
					{
						result = BigInteger.ModPow(a, b, c);
					}
					else if (result != BigInteger.ModPow(a, b, c))
					{
						throw new Exception();
					}
				}
			}
		}

		private static void BigInteger_TryParse(string path)
		{
			var bytes = File.ReadAllBytes(path);
			var number = new BigInteger(bytes);

			foreach (var format in formats)
			{
				if (number.TryFormat(buffer, out var size, format))
				{
					var span = buffer.AsSpan(0, size);

					if (!BigInteger.TryParse(span, NumberStyles.Any, null, out var parsed) || number != parsed)
					{
						throw new Exception();
					}

					span.Clear();
				}
			}
		}

		private static void BinaryFormatter_Deserialize(string path)
		{
			var formatter = new BinaryFormatter();

			try
			{
				var bytes = File.ReadAllBytes(path);
				var buffer = new byte[headerBytes.Length + bytes.Length];

				Array.Copy(headerBytes, buffer, headerBytes.Length);
				Array.Copy(bytes, 0, buffer, headerBytes.Length, bytes.Length);

				using (var stream = new MemoryStream(buffer))
				{
					formatter.Deserialize(stream);
				}
			}
			catch (ArgumentOutOfRangeException) { }
			catch (ArrayTypeMismatchException) { }
			catch (DecoderFallbackException) { }
			catch (ArgumentException) { }
			catch (FileLoadException) { }
			catch (FormatException) { }
			catch (IndexOutOfRangeException) { }
			catch (InvalidCastException) { }
			catch (IOException) { }
			catch (MemberAccessException) { }
			catch (NullReferenceException) { }
			catch (OutOfMemoryException) { }
			catch (OverflowException) { }
			catch (SerializationException) { }
		}

		private static void DataContractJsonSerializer_ReadObject(string path)
		{
			try
			{
				var serializer = new DataContractJsonSerializer(typeof(Obj));

				using (var file = File.OpenRead(path))
				{
					serializer.ReadObject(file);
				}
			}
			catch (ArgumentOutOfRangeException) { }
			catch (IndexOutOfRangeException) { }
			catch (SerializationException) { }
			catch (XmlException) { }
		}

		private static void DataContractSerializer_ReadObject(string path)
		{
			try
			{
				var serializer = new DataContractSerializer(typeof(Obj));

				using (var file = File.OpenRead(path))
				{
					serializer.ReadObject(file);
				}
			}
			catch (ArgumentNullException) { }
			catch (SerializationException) { }
			catch (XmlException) { }
		}

		private static void HttpUtility_UrlEncode(string path)
		{
			var text = File.ReadAllText(path);
			var encoded = HttpUtility.UrlEncode(text);
			var decoded = HttpUtility.UrlDecode(encoded);

			if (text != decoded)
			{
				throw new Exception();
			}
		}

		private static void PEReader_GetMetadataReader(string path)
		{
			try
			{
				using (var stream = File.OpenRead(path))
				using (var pe = new PEReader(stream))
				{
					pe.GetMetadataReader();
				}
			}
			catch (BadImageFormatException) { }
			catch (InvalidOperationException) { }
			catch (OverflowException) { }
		}

		private static void Regex_Match(string path)
		{
			var text = File.ReadAllText(path);

			foreach (var regex in regexes.Value)
			{
				regex.Match(text);
			}
		}

		private static void Utf8Parser_TryParseDateTime(string path)
		{
			Span<byte> from = File.ReadAllBytes(path);
			Span<byte> to = stackalloc byte[256];

			if (Utf8Parser.TryParse(from, out DateTime dt1, out _))
			{
				var format = 'O';

				if (!Utf8Formatter.TryFormat(dt1, to, out int written, format))
				{
					throw new Exception();
				}

				if (!Utf8Parser.TryParse(to.Slice(0, written), out DateTime dt2, out _, format))
				{
					throw new Exception();
				}

				if (dt1 != dt2)
				{
					throw new Exception();
				}
			}
		}

		private static void Utf8Parser_TryParseDouble(string path)
		{
			Span<byte> from = File.ReadAllBytes(path);
			Span<byte> to = stackalloc byte[256];

			if (Utf8Parser.TryParse(from, out double d1, out _))
			{
				if (!Utf8Formatter.TryFormat(d1, to, out int written))
				{
					throw new Exception();
				}

				if (!Utf8Parser.TryParse(to.Slice(0, written), out double d2, out _))
				{
					throw new Exception();
				}
			}
		}

		private static void Utf8Parser_TryParseTimeSpan(string path)
		{
			Span<byte> from = File.ReadAllBytes(path);
			Span<byte> to = stackalloc byte[256];

			if (Utf8Parser.TryParse(from, out TimeSpan t1, out _))
			{
				var format = 'c';

				if (!Utf8Formatter.TryFormat(t1, to, out int written, format))
				{
					throw new Exception();
				}

				if (!Utf8Parser.TryParse(to.Slice(0, written), out TimeSpan t2, out _, format))
				{
					throw new Exception();
				}

				if (t1 != t2)
				{
					throw new Exception();
				}
			}
		}

		private static void XmlReader_Create(string path)
		{
			try
			{
				using (var stream = File.OpenRead(path))
				using (var xml = XmlReader.Create(stream))
				{
					while (xml.Read()) { }
				}
			}
			catch (IndexOutOfRangeException) { }
			catch (XmlException) { }
		}

		private static void XmlSerializer_Deserialize(string path)
		{
			var serializer = new XmlSerializer(typeof(Obj));

			try
			{
				using (var stream = File.OpenRead(path))
				{
					serializer.Deserialize(stream);
				}
			}
			catch (IndexOutOfRangeException) { }
			catch (InvalidOperationException) { }
			catch (XmlException) { }
		}

		private static void XmlSerializer_Deserialize(ReadOnlySpan<byte> data)
		{
			var serializer = new XmlSerializer(typeof(Obj));

			try
			{
				using (var stream = new MemoryStream(data.ToArray()))
				{
					serializer.Deserialize(stream);
				}
			}
			catch (IndexOutOfRangeException) { }
			catch (InvalidOperationException) { }
			catch (XmlException) { }
		}

		private static void ZipArchive_Entries(string path)
		{
			try
			{
				using (var stream = File.OpenRead(path))
				using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
				{
					foreach (var entry in archive.Entries) { }
				}
			}
			catch (InvalidDataException) { }
			catch (IOException) { }
		}
	}
}
