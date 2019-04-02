using SharpFuzz;

namespace System.Private.CoreLib.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			switch (args[0])
			{
				case "Double.TryParse": Fuzzer.Run(Double_TryParse); return;
				default: throw new ArgumentException($"Unknown fuzzing function: {args[0]}");
			}
		}

		private static void Double_TryParse(string text)
		{
			if (Double.TryParse(text, out var d1))
			{
				var s = d1.ToString("R");
				var d2 = Double.Parse(s);

				if (d1 != d2)
				{
					throw new Exception();
				}
			}
		}
	}
}
