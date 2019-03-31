using System;
using SharpFuzz;

namespace NUglify.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(text =>
			{
				try
				{
					Uglify.Js(text);
				}
				catch (ArgumentOutOfRangeException) { }
				catch (IndexOutOfRangeException) { }
				catch (NullReferenceException) { }
			});
		}
	}
}
