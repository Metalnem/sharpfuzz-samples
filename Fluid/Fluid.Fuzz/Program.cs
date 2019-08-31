using System;
using System.Collections.Generic;
using SharpFuzz;

namespace Fluid.Fuzz
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

			Fuzzer.OutOfProcess.Run(text =>
			{
				try
				{
					if (FluidTemplate.TryParse(text, out var template))
					{
						TemplateContext.GlobalMemberAccessStrategy.Register<User>();
						template.Render(new TemplateContext { Model = user });
					}
				}
				catch (ArgumentOutOfRangeException) { }
				catch (ArgumentException) { }
				catch (DivideByZeroException) { }
				catch (NullReferenceException) { }
			});
		}
	}
}
