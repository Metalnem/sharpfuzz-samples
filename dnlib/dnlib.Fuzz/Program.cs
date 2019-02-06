using System;
using dnlib.DotNet;
using dnlib.IO;
using SharpFuzz;

namespace dnlib.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					ModuleDefMD.Load(args[0]);
				}
				catch (BadImageFormatException) { }
				catch (DataReaderException) { }
			});
		}
	}
}
