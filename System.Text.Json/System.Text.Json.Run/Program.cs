using System.IO;
using Fuzzing.Text.Json;

namespace System.Text.Json.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var file = File.OpenRead(args[0]))
			{
				JsonDocument.Parse(file);
			}
		}
	}
}
