using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Web
{
    public static class WebHostExtensions
    {
        public static IWebHostBuilder UseStrategyRunnerStartup(this IWebHostBuilder webHost, string[] args)
        {
            if (webHost == null)
            {
                throw new ArgumentNullException(nameof(webHost));
            }

            return webHost.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddCommandLine(args);
            }).UseStartup<Startup>();
        }
    }
}
