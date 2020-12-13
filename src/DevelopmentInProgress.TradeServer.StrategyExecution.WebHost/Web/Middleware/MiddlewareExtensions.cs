using Microsoft.AspNetCore.Builder;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Web.Middleware
{
    public static class MiddlewareExtensions
    {
        internal static IApplicationBuilder UseRunStrategyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RunStrategyMiddleware>();
        }

        internal static IApplicationBuilder UseUpdateStrategyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UpdateStrategyMiddleware>();
        }

        internal static IApplicationBuilder UseIsStrategyRunningMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IsStrategyRunningMiddleware>();
        }

        internal static IApplicationBuilder UseStopStrategyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<StopStrategyMiddleware>();
        }

        internal static IApplicationBuilder UsePingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PingMiddleware>();
        }
    }
}