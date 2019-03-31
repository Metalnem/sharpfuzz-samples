using SharpFuzz;

namespace WebMarkupMin.Core.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(text =>
			{
				new HtmlMinifier().Minify(text);
			});
		}
	}
}
