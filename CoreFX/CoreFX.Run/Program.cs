using System.IO;
using System.Xml;

namespace CoreFX.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var stream = File.OpenRead(args[0]))
			using (var xml = XmlReader.Create(stream))
			{
				while (xml.Read()) { }
			}
		}
	}
}
