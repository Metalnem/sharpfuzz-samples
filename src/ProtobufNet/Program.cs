﻿using ProtoBuf;
using SharpFuzz;

namespace ProtobufNet.Fuzz
{
	[ProtoContract]
	public class Person
	{
		[ProtoMember(1)] public int Id { get; set; }
		[ProtoMember(2)] public string Name { get; set; }
		[ProtoMember(3)] public Address Address { get; set; }
	}

	[ProtoContract]
	public class Address
	{
		[ProtoMember(1)] public string Line1 { get; set; }
		[ProtoMember(2)] public string Line2 { get; set; }
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(stream =>
			{
				try
				{
					Serializer.Deserialize<Person>(stream);
				}
				catch { }
			});
		}
	}
}
