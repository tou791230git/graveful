using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.Loader;

namespace TestGraceful
{
	class Program
	{
		static void Main(string[] args)
		{

			AssemblyLoadContext.Default.Unloading += ctx =>
			{
				System.Console.WriteLine("--------------------");
			};

			Console.WriteLine("Hello World!");
			var host = CreateHostBuilder(args).Build();
			host.Run();
		}
		private static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder()
				.ConfigureAppConfiguration((hostContext, configbuilder) =>
				{
					var ienv = hostContext.HostingEnvironment;
					configbuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
						  .AddJsonFile($"appsettings.{ienv.EnvironmentName}.json", optional: true, reloadOnChange: true)
						  .AddEnvironmentVariables()
						  .Build();

				}).ConfigureServices((hostContext, services) =>
				{
					services.Configure<IConfiguration>(hostContext.Configuration);
					services.AddLogging();
					services.AddHostedService<ShutdownService>();

					services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(15));
				}).UseConsoleLifetime();
		}
	}
}
