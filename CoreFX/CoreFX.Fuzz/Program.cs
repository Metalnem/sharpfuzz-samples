using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
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

		private static readonly Dictionary<string, Action<string>> fuzzers =
			new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
			{
				{ "BigInteger.Multiply", BigInteger_Multiply },
				{ "BigInteger.TryParse", BigInteger_TryParse },
				{ "DataContractJsonSerializer.ReadObject", DataContractJsonSerializer_ReadObject },
				{ "DataContractSerializer.ReadObject", DataContractSerializer_ReadObject },
				{ "HttpUtility.UrlEncode", HttpUtility_UrlEncode },
				{ "PEReader.GetMetadataReader", PEReader_GetMetadataReader },
				{ "Regex.Match", Regex_Match },
				{ "XmlReader.Create", XmlReader_Create },
				{ "XmlSerializer.Deserialize", XmlSerializer_Deserialize },
				{ "ZipArchive.Entries", ZipArchive_Entries }
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

		private static void BigInteger_Multiply(string path)
		{
			var bytes = File.ReadAllBytes(path);
			var span = bytes.AsSpan(0, Math.Min(bytes.Length, 8192));

			var left = new BigInteger(span.Slice(0, span.Length / 2).ToArray());
			var right = new BigInteger(span.Slice(span.Length / 2).ToArray());

			if (left.IsZero)
			{
				left = new BigInteger(1);
			}

			if (right.IsZero)
			{
				right = new BigInteger(1);
			}

			var absLeft = BigInteger.Abs(left);
			var absRight = BigInteger.Abs(right);

			var product = BigInteger.Multiply(left, right);

			var gcdLeft = BigInteger.GreatestCommonDivisor(product, left);
			var gcdRight = BigInteger.GreatestCommonDivisor(product, right);

			if (absLeft != gcdLeft || absRight != gcdRight)
			{
				throw new Exception();
			}

			var divLeft = BigInteger.Divide(product, left);
			var divRight = BigInteger.Divide(product, right);

			if (left != divRight || right != divLeft)
			{
				throw new Exception();
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
