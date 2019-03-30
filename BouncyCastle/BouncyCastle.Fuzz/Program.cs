using System;
using System.IO;
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
					using (var memory = new MemoryStream())
					{
						stream.CopyTo(memory);
						memory.Seek(0, SeekOrigin.Begin);

						using (var asn = new Asn1InputStream(memory))
						{
							while (asn.ReadObject() != null) { }
						}
					}
				}
				catch (ArgumentException) { }
				catch (Asn1Exception) { }
				catch (Asn1ParsingException) { }
				catch (InvalidCastException) { }
				catch (InvalidOperationException) { }
				catch (IOException) { }
			});
		}
	}
}
