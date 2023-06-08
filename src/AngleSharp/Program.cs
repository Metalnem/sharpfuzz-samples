using AngleSharp.Html.Parser;
using SharpFuzz;

namespace AngleSharp.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(s =>
			{
				try
				{
					var parser = new HtmlParser();
					parser.ParseDocument(s);
				}
				catch { }
			});
		}
	}
}
