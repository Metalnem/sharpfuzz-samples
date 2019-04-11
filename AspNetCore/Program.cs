using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SharpFuzz;

namespace AspNetCore.Fuzz
{
	public class Program
	{
		private static readonly byte[] buffer = new byte[1024];

		public static unsafe void Main(string[] args)
		{
			fixed (byte* trace = new byte[65536])
			{
				SharpFuzz.Common.Trace.SharedMem = trace;

				WebHost.CreateDefaultBuilder(args)
					.UseKestrel(options => options.Limits.RequestHeadersTimeout = TimeSpan.FromMilliseconds(100))
					.UseStartup<Startup>()
					.Build()
					.Run();
			}
		}

		private static void Fuzz()
		{
			Fuzzer.Run(input =>
			{
				using (var client = new TcpClient("localhost", 5000))
				using (var output = client.GetStream())
				{
					input.CopyTo(output);
					output.Read(buffer, 0, buffer.Length);
				}
			});
		}

		private class Startup
		{
			public void Configure(IApplicationBuilder app, IApplicationLifetime lifetime)
			{
				app.Run(async context => await context.Response.WriteAsync(String.Empty));
				lifetime.ApplicationStarted.Register(() => Task.Run((Action)Fuzz));
			}
		}
	}
}
