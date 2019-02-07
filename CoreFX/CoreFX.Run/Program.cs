using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Xml;

namespace CoreFX.Run
{
	public class Program
	{
		private static readonly Dictionary<string, Action<string>> runners =
			new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
			{
				{ "XmlReader.Create", XmlReader_Create },
				{ "PEReader.GetMetadataReader", PEReader_GetMetadataReader }
			};

		public static void Main(string[] args)
		{
			var runner = runners[args[1]];
			var path = args[0];

			runner(path);
		}

		private static void XmlReader_Create(string path)
		{
			using (var stream = File.OpenRead(path))
			using (var xml = XmlReader.Create(stream))
			{
				while (xml.Read()) { }
			}
		}

		private static void PEReader_GetMetadataReader(string path)
		{
			using (var stream = File.OpenRead(path))
			using (var pe = new PEReader(stream))
			{
				pe.GetMetadataReader();
			}
		}
	}
}
