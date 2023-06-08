using SharpFuzz;

namespace MessagePack.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(stream =>
			{
				try
				{
					MessagePackSerializer.Deserialize<dynamic>(stream);
				}
				catch { }
			});
		}
	}
}
