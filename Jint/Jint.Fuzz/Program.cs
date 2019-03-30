using System;
using System.IO;
using System.Text;
using Esprima;
using Jint.Runtime;
using SharpFuzz;

namespace Jint.Fuzz
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
						new Engine(SetOptions).Execute(text);
					}
				}
				catch (ArgumentOutOfRangeException) { }
				catch (ArgumentException) { }
				catch (IndexOutOfRangeException) { }
				catch (InvalidCastException) { }
				catch (InvalidOperationException) { }
				catch (JavaScriptException) { }
				catch (NullReferenceException) { }
				catch (OverflowException) { }
				catch (ParserException) { }
				catch (RecursionDepthOverflowException) { }
				catch (TimeoutException) { }
			});
		}

		private static void SetOptions(Options options)
		{
			options.LimitRecursion(32).TimeoutInterval(TimeSpan.FromSeconds(2));
		}
	}
}
