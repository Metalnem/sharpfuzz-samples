﻿using System;
using System.IO;
using Esprima;
using SharpFuzz;

namespace esprima.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(() =>
			{
				try
				{
					var text = File.ReadAllText(args[0]);
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
