using System;
using GraphQLParser;
using GraphQLParser.Exceptions;
using SharpFuzz;

namespace GraphQL.Parser.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(text =>
			{
				try
				{
					var parser = new Parser(new Lexer());
					parser.Parse(new Source(text));
				}
				catch (ArgumentOutOfRangeException) { }
				catch (GraphQLSyntaxErrorException) { }
			});
		}
	}
}
