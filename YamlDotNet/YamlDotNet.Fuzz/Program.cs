using System;
using System.IO;
using System.Text;
using SharpFuzz;
using YamlDotNet.Core;
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
					using (var reader = new StreamReader(stream, Encoding.UTF8, false, 4096, true))
					{
						new YamlStream().Load(reader);
					}
				}
				catch (ArgumentException) { }
				catch (YamlException) { }
			});
		}
	}
}
