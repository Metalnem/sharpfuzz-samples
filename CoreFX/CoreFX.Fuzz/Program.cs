using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Xml;
using SharpFuzz;

namespace CoreFX.Fuzz
{
	public class Program
	{
		private static readonly Dictionary<string, Action<string>> fuzzers =
			new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
			{
				{ "XmlReader.Create", XmlReader_Create },
				{ "PEReader.GetMetadataReader", PEReader_GetMetadataReader }
			};

		public static void Main(string[] args)
		{
			var fuzzer = fuzzers[args[1]];
			var path = args[0];

			Fuzzer.OutOfProcess.Run(() => fuzzer(path));
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
	}
}
