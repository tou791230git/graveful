using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestGraceful
{
	public class ShutdownService : IHostedService
    {
        private bool pleaseStop;
        private Task BackgroundTask;
        private readonly IHostApplicationLifetime applicationLifetime;

        public ShutdownService(IHostApplicationLifetime applicationLifetime)
        {
            this.applicationLifetime = applicationLifetime;
        }

        public Task StartAsync(CancellationToken _)
        {

			Log.Logger = new LoggerConfiguration()
				.WriteTo.Seq("http://172.16.64.103:5300")
				.CreateLogger();
            Log.Information("Test!!");

            AssemblyLoadContext.Default.Unloading += ctx =>
            {
                System.Console.WriteLine("XXXXXXXXXXXXXXXXXXX");
                Log.Information("XXXXXXXXXXXXXXXXXXX");
            };

            Console.WriteLine("Starting service:" + DateTime.Now);

            this.applicationLifetime.ApplicationStarted.Register(() =>
            {
                Console.WriteLine("ApplicationStarted");
            });

            this.applicationLifetime.ApplicationStopping.Register(() =>
            {
                Console.WriteLine("ApplicationStopping");

            });
            this.applicationLifetime.ApplicationStopped.Register(() =>
            {

                Console.WriteLine("ApplicationStopped SIGTERM received");
                Log.Information("ApplicationStopped SIGTERM received");
				for (int i = 0; i < 3; i++)
				{
                    Thread.Sleep(1_000);
                    Log.Information("Sleep: "+i);
                    Console.WriteLine("Sleep: " + i);
                }
               
                Log.Information("Termination delay complete, continuing stopping process");
                Console.WriteLine("Termination delay complete, continuing stopping process");
               // Environment.Exit(0);
            });

			BackgroundTask = Task.Run(async () =>
			{
                int a = 0;
				while (!pleaseStop)
				{
                    Console.WriteLine(a++);
                    Log.Information(a.ToString());
                    await Task.Delay(5000);
				}

				Console.WriteLine("Background task gracefully stopped");
			});

			return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping service");

            pleaseStop = true;
            await BackgroundTask;

            Console.WriteLine("Service stopped");
        }
    }
}
