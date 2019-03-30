using System;
using System.IO;
using System.Text;
using Jint;
using Jint.Runtime;
using SharpFuzz;

namespace Jurassic.Fuzz
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

						if (RunJint(text))
						{
							var engine = new ScriptEngine();
							engine.Execute(text);
						}
					}
				}
				catch (JavaScriptException) { }
			});
		}

		private static bool RunJint(string code)
		{
			try { new Engine(SetOptions).Execute(code); }
			catch (RecursionDepthOverflowException) { return false; }
			catch (TimeoutException) { return false; }
			catch (Exception) { return true; }

			return true;
		}

		private static void SetOptions(Options options)
		{
			options.LimitRecursion(16).TimeoutInterval(TimeSpan.FromSeconds(1));
		}
	}
}
