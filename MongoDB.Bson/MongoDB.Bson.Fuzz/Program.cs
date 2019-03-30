using System;
using System.IO;
using System.Text;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using SharpFuzz;

namespace MongoDB.Bson.Fuzz
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

						using (var bson = new BsonBinaryReader(memory))
						{
							BsonSerializer.Deserialize(bson, typeof(object));
						}
					}
				}
				catch (ArgumentOutOfRangeException) { }
				catch (DecoderFallbackException) { }
				catch (ArgumentException) { }
				catch (FormatException) { }
				catch (IndexOutOfRangeException) { }
				catch (IOException) { }
				catch (OutOfMemoryException) { }
			});
		}
	}
}
