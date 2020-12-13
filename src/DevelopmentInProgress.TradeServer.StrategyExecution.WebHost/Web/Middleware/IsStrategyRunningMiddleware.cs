using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.TradeStrategy;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Web.Middleware
{
    public class IsStrategyRunningMiddleware
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        public IsStrategyRunningMiddleware(RequestDelegate next)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        public async Task Invoke(HttpContext context, ITradeStrategyCacheManager tradeStrategyCacheManager)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (tradeStrategyCacheManager == null)
            {
                throw new ArgumentNullException(nameof(tradeStrategyCacheManager));
            }

            try
            {
                var json = context.Request.Form["strategyparameters"];

                var strategyParameters = JsonConvert.DeserializeObject<StrategyParameters>(json);

                if (tradeStrategyCacheManager.TryGetTradeStrategy(strategyParameters.StrategyName, out ITradeStrategy tradeStrategy))
                {
                    await context.Response.WriteAsync("YES").ConfigureAwait(false);
                }
                else
                {
                    await context.Response.WriteAsync("NO").ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await response.WriteAsync(JsonConvert.SerializeObject(ex)).ConfigureAwait(false);
            }
        }
    }
}
