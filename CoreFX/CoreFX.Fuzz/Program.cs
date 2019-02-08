using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Web;
using System.Xml;
using SharpFuzz;

namespace CoreFX.Fuzz
{
	public class Program
	{
		private static readonly Dictionary<string, Action<string>> fuzzers =
			new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
			{
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
