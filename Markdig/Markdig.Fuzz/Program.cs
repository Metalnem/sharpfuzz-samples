using System.IO;
using System.Text;
using SharpFuzz;

namespace Markdig.Fuzz
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
					var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
					Markdown.ToHtml(text, pipeline);
				}
			});
		}
	}
}
