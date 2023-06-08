using Org.BouncyCastle.Asn1;
using SharpFuzz;

namespace BouncyCastle.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(stream =>
			{
				try
				{
					using (var asn = new Asn1InputStream(stream))
					{
						while (asn.ReadObject() != null) { }
					}
				}
				catch { }
			});
		}
	}
}
