using DevelopmentInProgress.Socket.Extensions;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.TradeStrategy;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Server;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Strategy;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Web.HostedService;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Web.Middleware;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Server;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using DevelopmentInProgress.TradeView.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Web
{
    public class Startup
    {
        private readonly ILogger<Startup> logger;
        private IServerManager serverManager;

        public Startup(IHostingEnvironment env, IConfiguration configuration, ILogger<Startup> logger)
        {
            if (env == null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables()
                .AddConfiguration(configuration);
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            int maxDegreeOfParallelism = 5;

            if (Convert.ToInt32(Configuration["p"], CultureInfo.InvariantCulture) > 0)
            {
                maxDegreeOfParallelism = Convert.ToInt32(Configuration["p"], CultureInfo.InvariantCulture);
            }

            // Get the container to create the ServerMonitor instance  
            // so the container automatically handles disposing it.
            services.AddSingleton<IServerMonitor>(sm => new ServerMonitor
            {
                Started = DateTime.Now,
                StartedBy = Environment.UserName,
                Name = Configuration["s"].ToString(),
                Uri = new Uri(Configuration["u"].ToString()),
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            });

            services.AddSingleton<IStrategyRunnerActionBlock, StrategyRunnerActionBlock>();
            services.AddTransient<IStrategyRunner, StrategyRunner>();
            services.AddSingleton<IServerNotificationPublisherContext, ServerNotificationPublisherContext>();
            services.AddSingleton<IServerNotificationPublisher, ServerNotificationPublisher>();
            services.AddSingleton<IBatchNotification<ServerNotification>, ServerBatchNotificationPublisher>();
            services.AddSingleton<IStrategyNotificationPublisherContext, StrategyNotificationPublisherContext>();
            services.AddSingleton<IStrategyNotificationPublisher, StrategyNotificationPublisher>();
            services.AddSingleton<IBatchNotificationFactory<StrategyNotification>, StrategyBatchNotificationFactory>();
            services.AddSingleton<IExchangeApiFactory, ExchangeApiFactory>();
            services.AddSingleton<IExchangeService, ExchangeService>();
            services.AddSingleton<IExchangeSubscriptionsCacheFactory, ExchangeSubscriptionsCacheFactory>();
            services.AddSingleton<ISubscriptionsCacheManager, SubscriptionsCacheManager>();
            services.AddHostedService<StrategyRunnerBackgroundService>();
            services.AddSingleton<ITradeStrategyCacheManager, TradeStrategyCacheManager>();

            services.AddSocket<StrategyNotificationHub>();
            services.AddSocket<ServerNotificationHub>();

            services.AddSingleton<IServerManager, ServerManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if(applicationLifetime == null)
            {
                throw new ArgumentNullException(nameof(applicationLifetime));
            }

            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            app.UseSocket<StrategyNotificationHub>("/notificationhub");
            app.UseSocket<ServerNotificationHub>("/serverhub");

            app.Map("/runstrategy", HandleRun);
            app.Map("/updatestrategy", HandleUpdate);
            app.Map("/stopstrategy", HandleStop);
            app.Map("/isstrategyrunning", HandleIsStrategyRunning);
            app.Map("/ping", HandlePing);

            // Create instance of the Server Manager.
            serverManager = app.ApplicationServices.GetRequiredService<IServerManager>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        private void OnShutdown()
        {
            try
            {
                serverManager.Shutdown();
            }
            catch(Exception ex)
            {
                logger.Log(LogLevel.Error, ex.ToString());
            }
        }

        private static void HandleRun(IApplicationBuilder app)
        {
            app.UseRunStrategyMiddleware();
        }

        private static void HandleUpdate(IApplicationBuilder app)
        {
            app.UseUpdateStrategyMiddleware();
        }

        private static void HandleIsStrategyRunning(IApplicationBuilder app)
        {
            app.UseIsStrategyRunningMiddleware();
        }

        private static void HandleStop(IApplicationBuilder app)
        {
            app.UseStopStrategyMiddleware();
        }

        private static void HandlePing(IApplicationBuilder app)
        {
            app.UsePingMiddleware();
        }
    }
}
