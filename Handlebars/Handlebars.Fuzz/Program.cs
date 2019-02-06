using System;
using System.Collections.Generic;
using System.IO;
using HandlebarsDotNet;
using SharpFuzz;

namespace Handlebars.Fuzz
{
	public class User
	{
		public string String { get; set; }
		public int Integer { get; set; }
		public List<double> Doubles { get; set; }
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			var user = new User
			{
				String = "ABC",
				Integer = 123,
				Doubles = new List<double> { 1.1, 2.2, 3.3 }
			};

			Fuzzer.OutOfProcess.Run(() =>
			{
				try
				{
					var text = File.ReadAllText(args[0]);
					var template = HandlebarsDotNet.Handlebars.Compile(text);

					template(user);
				}
				catch (ArgumentException) { }
				catch (HandlebarsException) { }
				catch (InvalidCastException) { }
				catch (InvalidOperationException) { }
				catch (NullReferenceException) { }
			});
		}
	}
}
