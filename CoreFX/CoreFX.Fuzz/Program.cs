using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Web;
using System.Xml;
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
				{ "HttpUtility.UrlEncode", HttpUtility_UrlEncode },
				{ "PEReader.GetMetadataReader", PEReader_GetMetadataReader },
				{ "XmlReader.Create", XmlReader_Create },
				{ "ZipArchive.Entries", ZipArchive_Entries }
			};

		public static void Main(string[] args)
		{
			var fuzzer = fuzzers[args[1]];
			var path = args[0];

			Fuzzer.OutOfProcess.Run(() => fuzzer(path));
		}

		private static void BigInteger_Multiply(string path)
		{
			var bytes = File.ReadAllBytes(path);

			var left = new BigInteger(bytes.AsSpan(0, bytes.Length / 2).ToArray());
			var right = new BigInteger(bytes.AsSpan(bytes.Length / 2).ToArray());

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
