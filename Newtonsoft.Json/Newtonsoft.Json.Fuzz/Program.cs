using System;
using System.IO;
using System.Text;
using SharpFuzz;

namespace Newtonsoft.Json.Fuzz
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
						var text = reader.ReadToEnd();
						JsonConvert.DeserializeObject(text);
					}
				}
				catch (ArgumentException) { }
				catch (JsonReaderException) { }
				catch (JsonSerializationException) { }
				catch (JsonWriterException) { }
				catch (NullReferenceException) { }
			});
		}
	}
}
