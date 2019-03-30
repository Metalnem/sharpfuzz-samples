using System;
using SharpFuzz;

namespace MsgPack.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(stream =>
			{
				try
				{
					Unpacking.UnpackObject(stream);
				}
				catch (InvalidMessagePackStreamException) { }
				catch (MessageNotSupportedException) { }
				catch (MessageTypeException) { }
				catch (OutOfMemoryException) { }
				catch (OverflowException) { }
				catch (UnpackException) { }
			});
		}
	}
}
