using SharpFuzz;

namespace GraphQL.Parser.Fuzz
{
    public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(s =>
			{
				try
				{
					GraphQLParser.Parser.Parse(s);
				}
				catch { }
			});
		}
	}
}
