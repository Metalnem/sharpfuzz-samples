using System.IO;
using SharpFuzz;

namespace SixLabors.ImageSharp.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(() =>
			{
				try
				{
					var bytes = File.ReadAllBytes(args[0]);

					bytes[0] = 0xff;
					bytes[1] = 0xd8;

					Image.Load(bytes);
				}
				catch (ImageFormatException) { }
			});
		}
	}
}
