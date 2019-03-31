using System;
using SharpFuzz;

namespace Newtonsoft.Json.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(text =>
			{
				try
				{
					JsonConvert.DeserializeObject(text);
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
