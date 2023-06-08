using System.IO;
using SharpFuzz;
using YamlDotNet.RepresentationModel;

namespace YamlDotNet.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(stream =>
			{
				try
				{
					using (var reader = new StreamReader(stream))
					{
						new YamlStream().Load(reader);
					}
				}
				catch  { }
			});
		}
	}
}
