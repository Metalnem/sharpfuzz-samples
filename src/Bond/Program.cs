using Bond.IO.Unsafe;
using Bond.Protocols;
using examples.serialization;
using SharpFuzz;

namespace Bond.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(stream =>
			{
				try
				{
					var input = new InputStream(stream);
					var reader = new CompactBinaryReader<InputStream>(input);

					Deserialize<Struct>.From(reader);
				}
				catch { }
			});
		}
	}
}
