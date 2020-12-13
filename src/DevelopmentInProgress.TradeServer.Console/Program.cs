using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using System;
using System.Linq;

namespace DevelopmentInProgress.TradeServer.Console
{
    class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        static void Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("Logs\\DevelopmentInProgress.TradeServer.Console-.log", rollingInterval: RollingInterval.Day)
                    .WriteTo.Console()
                    .CreateLogger();

                Log.Information($"Running as {Environment.UserName}");

                if (args == null
                    || args.Length.Equals(0))
                {
                    Log.Warning($"No args. Use defaults...");

                    args = new[]
                    {
                        $"s=TradeServer",
                        "u=http://localhost:5500"
                    };
                }
                else if (InvalidArgs(args))
                {
                    Log.Error($"Invalid args");

                    foreach (var arg in args)
                    {
                        Log.Error($"{arg}");
                    }

                    Log.Error($"You must provide the following args:");
                    Log.Error($"--s=YourServerName");
                    Log.Error($"--u=http://localhost:5500");

                    return;
                }

                Log.Information($"args");
                foreach (var arg in args)
                {
                    Log.Information($"{arg}");
                }

                var url = args.First(a => a.StartsWith("u=", StringComparison.Ordinal)).Split("=")[1];

                Log.Information("Launching DevelopmentInProgress.TradeServer.Console");

                var webHost = WebHost.CreateDefaultBuilder()
                    .UseUrls(url)
                    .UseStrategyRunnerStartup(args)
                    .UseSerilog()
                    .Build();

                var task = webHost.RunAsync();
                task.GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "DevelopmentInProgress.TradeServer.Console terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static bool InvalidArgs(string[] args)
        {
            var snMissing = true;
            for(int i = 0; i < args.Length; i++)
            {
                if(args[i].StartsWith("--s=", StringComparison.Ordinal))
                {
                    snMissing = false;
                    break;
                }
            }

            var urlMissing = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--u=", StringComparison.Ordinal))
                {
                    urlMissing = false;
                    break;
                }
            }

            if(snMissing || urlMissing)
            {
                return true;
            }

            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i][2..];
            }

            return false;
        }
    }
}
