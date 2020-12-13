using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Web.HostedService;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Web.Middleware
{
    public class RunStrategyMiddleware
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        public RunStrategyMiddleware(RequestDelegate next)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        public async Task Invoke(HttpContext context, IStrategyRunner strategyRunner, IStrategyRunnerActionBlock strategyRunnerActionBlock)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (strategyRunner == null)
            {
                throw new ArgumentNullException(nameof(strategyRunner));
            }

            if (strategyRunnerActionBlock == null)
            {
                throw new ArgumentNullException(nameof(strategyRunnerActionBlock));
            }

            try
            {
                var json = context.Request.Form["strategy"];

                var strategy = JsonConvert.DeserializeObject<Strategy>(json);

                var downloadsPath = Path.Combine(Directory.GetCurrentDirectory(), "downloads", Guid.NewGuid().ToString());

                if (!Directory.Exists(downloadsPath))
                {
                    Directory.CreateDirectory(downloadsPath);
                }

                if (context.Request.HasFormContentType)
                {
                    var form = context.Request.Form;

                    var downloads = from f
                                    in form.Files
                                    select Download(f, downloadsPath);

                    await Task.WhenAll(downloads.ToArray()).ConfigureAwait(false);
                }

                var strategyRunnerActionBlockInput = new StrategyRunnerActionBlockInput
                {
                    StrategyRunner = strategyRunner,
                    Strategy = strategy,
                    DownloadsPath = downloadsPath
                };

                await strategyRunnerActionBlock.RunStrategyAsync(strategyRunnerActionBlockInput).ConfigureAwait(false);

                await context.Response.WriteAsync(json).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await response.WriteAsync(JsonConvert.SerializeObject(ex)).ConfigureAwait(false);
            }
        }

        private static async Task Download(IFormFile formFile, string downloadsPath)
        {
            using var fileStream = new FileStream(Path.Combine(downloadsPath, formFile.Name), FileMode.Create);
            await formFile.CopyToAsync(fileStream).ConfigureAwait(false);
        }
    }
}
