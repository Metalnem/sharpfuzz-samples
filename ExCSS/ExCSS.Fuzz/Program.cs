using System;
using SharpFuzz;

namespace ExCSS.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(stream =>
			{
				try
				{
					var parser = new StylesheetParser();
					parser.Parse(stream);
				}
				catch (ArgumentOutOfRangeException) { }
				catch (ParseException) { }
			});
		}
	}
}
