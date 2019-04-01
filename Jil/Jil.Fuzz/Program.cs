using System;
using System.IO;
using SharpFuzz;

namespace Jil.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(stream =>
			{
				try
				{
					using (var reader = new StreamReader(stream))
					{
						JSON.DeserializeDynamic(reader);
					}
				}
				catch (ArgumentException) { }
				catch (DeserializationException) { }
			});
		}
	}
}
