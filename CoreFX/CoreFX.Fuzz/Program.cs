using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using SharpFuzz;

namespace CoreFX.Fuzz
{
	public class Program
	{
		private static readonly byte[] byteBuffer = new byte[10_000_000];
		private static readonly char[] charBuffer = new char[8192];

		private static readonly string[] formats = new string[] { "C", "D", "G", "N", "R" };

		private static readonly FieldInfo squareThresholdField;
		private static readonly FieldInfo reducerThresholdField;

		private static readonly int[] squareThresholds = new int[] { 4, 32, Int32.MaxValue };
		private static readonly int[] reducerThresholds = new int[] { 0, 32, Int32.MaxValue };

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
				switch (args[0])
				{
					case "BigInteger.DivRem": Fuzzer.LibFuzzer.Run(BigInteger_DivRem); return;
					case "BigInteger.ModPow": Fuzzer.LibFuzzer.Run(BigInteger_ModPow); return;
					case "BigInteger.TryParse": Fuzzer.LibFuzzer.Run(BigInteger_TryParse); return;
					case "DataContractJsonSerializer.ReadObject": Fuzzer.LibFuzzer.Run(DataContractJsonSerializer_ReadObject); return;
					case "DataContractSerializer.ReadObject": Fuzzer.LibFuzzer.Run(DataContractSerializer_ReadObject); return;
					case "HttpUtility.UrlEncode": Fuzzer.LibFuzzer.Run(HttpUtility_UrlEncode); return;
					case "JsonDocument.Parse": Fuzzer.LibFuzzer.Run(JsonDocument_Parse); return;
					case "PEReader.GetMetadataReader": Fuzzer.LibFuzzer.Run(PEReader_GetMetadataReader); return;
					case "Regex.Match": Fuzzer.LibFuzzer.Run(Regex_Match); return;
					case "XmlReader.Create": Fuzzer.LibFuzzer.Run(XmlReader_Create); return;
					case "XmlSerializer.Deserialize": Fuzzer.LibFuzzer.Run(XmlSerializer_Deserialize); return;
					case "ZipArchive.Entries": Fuzzer.LibFuzzer.Run(ZipArchive_Entries); return;
					default: throw new ArgumentException($"Unknown fuzzing function: {args[0]}");
				}
			}

			switch (args[0])
			{
				case "BigInteger.DivRem": Run(BigInteger_DivRem); return;
				case "BigInteger.ModPow": Run(BigInteger_ModPow); return;
				case "BigInteger.TryParse": Run(BigInteger_TryParse); return;
				case "DataContractJsonSerializer.ReadObject": Run(DataContractJsonSerializer_ReadObject); return;
				case "DataContractSerializer.ReadObject": Run(DataContractSerializer_ReadObject); return;
				case "HttpUtility.UrlEncode": Run(HttpUtility_UrlEncode); return;
				case "JsonDocument.Parse": Run(JsonDocument_Parse); return;
				case "PEReader.GetMetadataReader": Run(PEReader_GetMetadataReader); return;
				case "Regex.Match": Run(Regex_Match); return;
				case "XmlReader.Create": Run(XmlReader_Create); return;
				case "XmlSerializer.Deserialize": Run(XmlSerializer_Deserialize); return;
				case "ZipArchive.Entries": Run(ZipArchive_Entries); return;
				default: throw new ArgumentException($"Unknown fuzzing function: {args[0]}");
			}
		}

		private static void Run(Action<Stream> action) => Fuzzer.OutOfProcess.Run(action);
		private static void Run(Action<String> action) => Fuzzer.OutOfProcess.Run(action);

		private static void BigInteger_DivRem(ReadOnlySpan<byte> data)
		{
			if (data.Length == 0 || data.Length > 8192)
			{
				return;
			}

			var span = data.Slice(1);
			var len = span.Length;

			var l1 = ((data[0] & 0x3f) * len) / 0x3f;
			var l2 = len - l1;

			var s1 = data[0] & 0x40;
			var s2 = data[0] & 0x80;

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

		private static void BigInteger_DivRem(Stream stream)
		{
			BigInteger_DivRem(ReadAllBytes(stream));
		}

		private static void BigInteger_ModPow(ReadOnlySpan<byte> data)
		{
			if (data.Length <= 2 || data.Length > 8192)
			{
				return;
			}

			var span = data.Slice(3);
			var len = span.Length;

			var l1 = (data[0] * len) / 255;
			var l2 = (data[1] * (len - l1)) / 255;
			var l3 = len - l1 - l2;

			var sa = data[2] & 1;
			var sc = data[2] & 2;

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

		private static void BigInteger_ModPow(Stream stream)
		{
			BigInteger_ModPow(ReadAllBytes(stream));
		}

		private static void BigInteger_TryParse(ReadOnlySpan<byte> data)
		{
			var number = new BigInteger(data);

			foreach (var format in formats)
			{
				if (number.TryFormat(charBuffer, out var size, format))
				{
					var span = charBuffer.AsSpan(0, size);

					if (!BigInteger.TryParse(span, NumberStyles.Any, null, out var parsed) || number != parsed)
					{
						throw new Exception();
					}

					span.Clear();
				}
			}
		}

		private static void BigInteger_TryParse(Stream stream)
		{
			BigInteger_TryParse(ReadAllBytes(stream));
		}

		private static void DataContractJsonSerializer_ReadObject(ReadOnlySpan<byte> data)
		{
			using (var stream = new MemoryStream(data.ToArray()))
			{
				DataContractJsonSerializer_ReadObject(stream);
			}
		}

		private static void DataContractJsonSerializer_ReadObject(Stream stream)
		{
			try
			{
				var serializer = new DataContractJsonSerializer(typeof(Obj));
				serializer.ReadObject(stream);
			}
			catch (ArgumentOutOfRangeException) { }
			catch (IndexOutOfRangeException) { }
			catch (SerializationException) { }
			catch (XmlException) { }
		}

		private static void DataContractSerializer_ReadObject(ReadOnlySpan<byte> data)
		{
			using (var stream = new MemoryStream(data.ToArray()))
			{
				DataContractSerializer_ReadObject(stream);
			}
		}

		private static void DataContractSerializer_ReadObject(Stream stream)
		{
			try
			{
				var serializer = new DataContractSerializer(typeof(Obj));
				serializer.ReadObject(stream);
			}
			catch (ArgumentNullException) { }
			catch (SerializationException) { }
			catch (XmlException) { }
		}

		private static void HttpUtility_UrlEncode(string text)
		{
			var encoded = HttpUtility.UrlEncode(text);
			var decoded = HttpUtility.UrlDecode(encoded);

			if (text != decoded)
			{
				throw new Exception();
			}
		}

		private static void HttpUtility_UrlEncode(ReadOnlySpan<byte> data)
		{
			var input = data.ToArray();
			var encoded = HttpUtility.UrlEncodeToBytes(input);
			var decoded = HttpUtility.UrlDecodeToBytes(encoded);

			if (!input.SequenceEqual(decoded))
			{
				throw new Exception();
			}
		}

		private static void JsonDocument_Parse(Stream stream)
		{
			try
			{
				JsonDocument.Parse(stream);
			}
			catch (JsonException) { }
		}

		private static void JsonDocument_Parse(ReadOnlySpan<byte> data)
		{
			try
			{
				JsonDocument.Parse(data.ToArray());
			}
			catch (JsonException) { }
		}

		private static unsafe void PEReader_GetMetadataReader(ReadOnlySpan<byte> data)
		{
			try
			{
				fixed (byte* ptr = data)
				{
					using (var pe = new PEReader(ptr, data.Length))
					{
						pe.GetMetadataReader();
					}
				}
			}
			catch (BadImageFormatException) { }
			catch (InvalidOperationException) { }
			catch (OverflowException) { }
		}

		private static void PEReader_GetMetadataReader(Stream stream)
		{
			PEReader_GetMetadataReader(ReadAllBytes(stream));
		}

		private static void Regex_Match(ReadOnlySpan<byte> data)
		{
			Regex_Match(Encoding.UTF8.GetString(data));
		}

		private static void Regex_Match(string text)
		{
			foreach (var regex in regexes.Value)
			{
				regex.Match(text);
			}
		}

		private static void XmlReader_Create(Stream stream)
		{
			try
			{
				using (var xml = XmlReader.Create(stream))
				{
					while (xml.Read()) { }
				}
			}
			catch (IndexOutOfRangeException) { }
			catch (XmlException) { }
		}

		private static void XmlReader_Create(ReadOnlySpan<byte> data)
		{
			try
			{
				using (var stream = new MemoryStream(data.ToArray()))
				using (var xml = XmlReader.Create(stream))
				{
					while (xml.Read()) { }
				}
			}
			catch (IndexOutOfRangeException) { }
			catch (XmlException) { }
		}

		private static void XmlSerializer_Deserialize(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(Obj));

			try
			{
				serializer.Deserialize(stream);
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

		private static void ZipArchive_Entries(ReadOnlySpan<byte> data)
		{
			using (var stream = new MemoryStream(data.ToArray()))
			{
				ZipArchive_Entries(stream);
			}
		}

		private static void ZipArchive_Entries(Stream stream)
		{
			try
			{
				using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
				{
					foreach (var entry in archive.Entries) { }
				}
			}
			catch (InvalidDataException) { }
			catch (IOException) { }
		}

		private static byte[] ReadAllBytes(Stream stream)
		{
			int size = stream.Read(byteBuffer, 0, byteBuffer.Length);

			if (size == byteBuffer.Length)
			{
				throw new Exception();
			}

			return byteBuffer.AsSpan(0, size).ToArray();
		}
	}
}
