using System;
using System.IO;
using System.Text;
using GraphQLParser;
using GraphQLParser.Exceptions;
using SharpFuzz;

namespace GraphQL_Parser.Fuzz
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
					{
						var text = reader.ReadToEnd();
						var parser = new Parser(new Lexer());
						parser.Parse(new Source(text));
					}
				}
				catch (ArgumentOutOfRangeException) { }
				catch (GraphQLSyntaxErrorException) { }
			});
		}
	}
}
