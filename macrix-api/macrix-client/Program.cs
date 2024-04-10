using macrix_client.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using macrix_client.Data;
using System.Net.Http.Headers;
using System.Net;
namespace macrix_client
{
    class Program
    {
        static void Main(string[] args)
        {

            var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddHttpClient()
            .AddSingleton<IActionsService, ActionsService>()
            .AddSingleton<IMacrixAPIService, MacrixAPIService>()
            .BuildServiceProvider();


            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Example log message");
            logger.LogDebug("Starting application");
            MainController Program = ActivatorUtilities.CreateInstance<MainController>(serviceProvider);
            Console.BufferWidth = 300;
            Console.BufferHeight = 300;
            Console.WindowHeight = 70;
            Console.WindowWidth = 250;
            Program.Start();
        }

    }
  
}
