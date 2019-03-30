using System;
using System.IO;
using System.Text;
using Esprima;
using SharpFuzz;

namespace esprima.Fuzz
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
						var parser = new JavaScriptParser(text);
						parser.ParseProgram();
					}
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
