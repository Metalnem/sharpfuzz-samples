using SharpFuzz;

namespace HtmlAgilityPack.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(stream =>
			{
				new HtmlDocument().Load(stream);
			});
		}
	}
}
