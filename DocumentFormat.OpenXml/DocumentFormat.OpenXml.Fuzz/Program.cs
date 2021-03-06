﻿using System;
using System.IO;
using System.Xml;
using DocumentFormat.OpenXml.Packaging;
using SharpFuzz;

namespace DocumentFormat.OpenXml.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(stream =>
			{
				try
				{
					SpreadsheetDocument.Open(stream, false);
				}
				catch (ArgumentException) { }
				catch (FileFormatException) { }
				catch (InvalidDataException) { }
				catch (InvalidOperationException) { }
				catch (IOException) { }
				catch (OpenXmlPackageException) { }
				catch (XmlException) { }
			});
		}
	}
}
