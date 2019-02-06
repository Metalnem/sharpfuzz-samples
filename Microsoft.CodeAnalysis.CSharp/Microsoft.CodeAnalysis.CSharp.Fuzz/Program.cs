using System.IO;
using SharpFuzz;

namespace Microsoft.CodeAnalysis.CSharp.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				var text = File.ReadAllText(args[0]);
				var tree = CSharpSyntaxTree.ParseText(text);

				var compilation = CSharpCompilation.Create(Path.GetRandomFileName())
					.WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication))
					.AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
					.AddSyntaxTrees(tree);

				using (var stream = new MemoryStream())
				{
					compilation.Emit(stream);
				}
			});
		}
	}
}
