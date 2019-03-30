using System;
using System.IO;
using System.Text;
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
					using (var reader = new StreamReader(stream, Encoding.UTF8, false, 4096, true))
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
