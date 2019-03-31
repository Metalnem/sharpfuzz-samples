using SharpFuzz;

namespace Markdig.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(text =>
			{
				var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
				Markdown.ToHtml(text, pipeline);
			});
		}
	}
}
