using System.IO;
using System.Text;
using SharpFuzz;

namespace WebMarkupMin.Core.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(stream =>
			{
				using (var reader = new StreamReader(stream, Encoding.UTF8, false, 4096, true))
				{
					var text = reader.ReadToEnd();
					new HtmlMinifier().Minify(text);
				}
			});
		}
	}
}
