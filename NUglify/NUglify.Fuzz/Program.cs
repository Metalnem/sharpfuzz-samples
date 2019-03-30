using System;
using System.IO;
using System.Text;
using SharpFuzz;

namespace NUglify.Fuzz
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
						Uglify.Js(text);
					}
				}
				catch (ArgumentOutOfRangeException) { }
				catch (IndexOutOfRangeException) { }
				catch (NullReferenceException) { }
			});
		}
	}
}
