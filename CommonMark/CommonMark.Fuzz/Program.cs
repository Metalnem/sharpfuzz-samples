using System.IO;
using System.Text;
using SharpFuzz;

namespace CommonMark.Fuzz
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
					using (var writer = new StringWriter())
					{
						CommonMarkConverter.Convert(reader, writer);
					}
				}
				catch (CommonMarkException) { }
			});
		}
	}
}
