using SharpFuzz;

namespace Markdig.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(s =>
			{
				try
				{
					var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
					Markdown.ToHtml(s, pipeline);
				}
				catch { }
			});
		}
	}
}
