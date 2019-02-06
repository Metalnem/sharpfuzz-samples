using System;
using System.IO;
using System.Xml;
using SharpFuzz;

namespace CoreFX.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(() =>
			{
				try
				{
					using (var stream = File.OpenRead(args[0]))
					using (var xml = XmlReader.Create(stream))
					{
						while (xml.Read()) { }
					}
				}
				catch (IndexOutOfRangeException) { }
				catch (XmlException) { }
			});
		}
	}
}
