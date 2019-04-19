using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Libfuzzer.Http;
using SharpFuzz;
using System.Threading;

namespace AspNetCore.Fuzz
{
	public class Program
	{
		private static readonly byte[] clientBuffer = new byte[10_000_000];
		private static readonly byte[] serverBuffer = new byte[10_000_000];

		public static unsafe void Main(string[] args)
		{
			fixed (byte* trace = new byte[65_536])
			{
				SharpFuzz.Common.Trace.SharedMem = trace;

				WebHost.CreateDefaultBuilder(args)
					.UseKestrel(options => options.Limits.RequestHeadersTimeout = TimeSpan.FromMilliseconds(100))
					.UseStartup<Startup>()
					.ConfigureLogging(logging => logging.ClearProviders())
					.Build()
					.Run();
			}
		}

		private static void Fuzz()
		{
			using (var client = new TcpClient("localhost", 5000))
			using (var stream = client.GetStream())
			{
				Fuzzer.LibFuzzer.Run(span =>
				{
					Request request = null;

					try
					{
						request = Request.Parser.ParseFrom(span.ToArray());
					}
					catch { }

					if (request != null)
					{
						Fuzz(request, stream);
					}
				});
			}
		}

		private static void Fuzz(Request request, NetworkStream stream)
		{
			var sb = new StringBuilder();

			sb.Append($"{GetMethod(request.Method)} {GetPath(request.Path)} HTTP/1.1\r\n");
			sb.Append($"Host: {GetHost(request.Host)}\r\n");
			sb.Append($"Connection: Keep-Alive\r\n");

			foreach (var header in request.Headers)
			{
				if (!String.Equals(header.Name, "Host", StringComparison.OrdinalIgnoreCase)
					&& !String.Equals(header.Name, "Connection", StringComparison.OrdinalIgnoreCase)
					&& !String.IsNullOrEmpty(header.Name)
					&& !String.IsNullOrEmpty(header.Value))
				{
					sb.Append($"{header.Name}: {header.Value}\r\n");
				}
			}

			sb.Append($"Content-Length: {request.Body.Length}\r\n\r\n");

			var bytes = Encoding.UTF8.GetBytes(sb.ToString());

			bytes.CopyTo(clientBuffer, 0);
			request.Body.CopyTo(clientBuffer, bytes.Length);

			stream.Write(clientBuffer, 0, bytes.Length + request.Body.Length);
			stream.Read(clientBuffer, 0, clientBuffer.Length);
		}

		private static string GetMethod(Method method)
		{
			switch (method)
			{
				case Method.Head: return "HEAD";
				case Method.Post: return "POST";
				case Method.Put: return "PUT";
				case Method.Delete: return "DELETE";
				case Method.Connect: return "CONNECT";
				case Method.Options: return "OPTIONS";
				case Method.Trace: return "TRACE";
				case Method.Patch: return "PATCH";
			}

			return "GET";
		}

		private static string GetPath(string path) => String.IsNullOrEmpty(path) ? "/" : path;
		private static string GetHost(string host) => String.IsNullOrEmpty(host) ? "localhost" : host;

		private class Startup
		{
			public void Configure(IApplicationBuilder app, IApplicationLifetime lifetime)
			{
				app.Run(async context =>
				{
					await context.Request.Body.ReadAsync(serverBuffer);
					await context.Response.WriteAsync(String.Empty);
				});

				lifetime.ApplicationStarted.Register(() => Task.Run((Action)Fuzz));
			}
		}
	}
}
