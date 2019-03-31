using System;
using Esprima;
using SharpFuzz;

namespace esprima.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(text =>
			{
				try
				{
					var parser = new JavaScriptParser(text);
					parser.ParseProgram();
				}
				catch (ArgumentOutOfRangeException) { }
				catch (IndexOutOfRangeException) { }
				catch (InvalidCastException) { }
				catch (InvalidOperationException) { }
				catch (OverflowException) { }
				catch (ParserException) { }
			});
		}
	}
}
