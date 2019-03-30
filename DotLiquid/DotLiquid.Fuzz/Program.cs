using System.Collections.Generic;
using System.IO;
using System.Text;
using DotLiquid.Exceptions;
using SharpFuzz;

namespace DotLiquid.Fuzz
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

			Fuzzer.OutOfProcess.Run(stream =>
			{
				try
				{
					using (var reader = new StreamReader(stream, Encoding.UTF8, false, 4096, true))
					{
						var text = reader.ReadToEnd();
						var template = Template.Parse(text);
						template.Render(Hash.FromAnonymousObject(user));
					}
				}
				catch (SyntaxException) { }
			});
		}
	}
}
