using System.IO;

namespace SixLabors.ImageSharp.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var bytes = File.ReadAllBytes(args[0]);

			bytes[0] = 0xff;
			bytes[1] = 0xd8;

			Image.Load(bytes);
		}
	}
}
