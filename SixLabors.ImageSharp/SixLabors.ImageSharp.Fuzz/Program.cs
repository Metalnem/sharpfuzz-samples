using SharpFuzz;

namespace SixLabors.ImageSharp.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(stream =>
			{
				try
				{
					Image.Load(stream);
				}
				catch (ImageFormatException) { }
			});
		}
	}
}
