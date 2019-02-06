using dnlib.DotNet;

namespace dnlib.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ModuleDefMD.Load(args[0]);
		}
	}
}
