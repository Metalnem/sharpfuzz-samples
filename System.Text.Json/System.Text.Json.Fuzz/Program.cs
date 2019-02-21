using System.IO;
using Fuzzing.Text.Json;
using SharpFuzz;

namespace System.Text.Json.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(() =>
			{
				using (var file = File.OpenRead(args[0]))
				{
					try
					{
						JsonDocument.Parse(file);
					}
					catch (InvalidOperationException) { }
				}
			});
		}
	}
}
