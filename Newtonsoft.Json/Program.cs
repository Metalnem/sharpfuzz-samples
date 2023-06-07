using System;
using SharpFuzz;

namespace Newtonsoft.Json.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(s =>
			{
				try
				{
					JsonConvert.DeserializeObject(s);
				}
				catch { }
			});
		}
	}
}
