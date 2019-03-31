using SharpFuzz;

namespace CommonMark.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(text =>
			{
				try
				{
					CommonMarkConverter.Convert(text);
				}
				catch (CommonMarkException) { }
			});
		}
	}
}
