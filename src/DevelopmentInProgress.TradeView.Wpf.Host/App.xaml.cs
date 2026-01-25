using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Data;
using DevelopmentInProgress.TradeView.Data.File;
using DevelopmentInProgress.TradeView.Service;
using DevelopmentInProgress.TradeView.Wpf.Common.Cache;
using DevelopmentInProgress.TradeView.Wpf.Common.Chart;
using DevelopmentInProgress.TradeView.Wpf.Common.Helpers;
using DevelopmentInProgress.TradeView.Wpf.Common.Manager;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Configuration.Utility;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Navigation;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.RegionAdapters;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Strategies.Utility;
using DevelopmentInProgress.TradeView.Wpf.Trading.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using Prism.Unity;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using Xceed.Wpf.AvalonDock;

namespace DevelopmentInProgress.TradeView.Wpf.Host
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();

            Log.Information("All Prism modules initialized");

            var serverMonitorCache = Container.Resolve<IServerMonitorCache>();
            serverMonitorCache.StartObservingServers();

            _ = SubscribeAssetsAsync();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            var catalog = new ModuleCatalog();

            catalog.AddModule<DevelopmentInProgress.TradeView.Wpf.Configuration.ConfigurationModule>();
            catalog.AddModule<DevelopmentInProgress.TradeView.Wpf.Dashboard.DashboardModule>();
            catalog.AddModule<DevelopmentInProgress.TradeView.Wpf.Strategies.StrategiesModule>();
            catalog.AddModule<DevelopmentInProgress.TradeView.Wpf.Trading.TradingModule>();

            return catalog;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var serilogLoggerFactory = new SerilogLoggerFactory(Log.Logger, dispose: false);

            containerRegistry.RegisterInstance<Serilog.ILogger>(Log.Logger);
            containerRegistry.RegisterInstance<ILoggerFactory>(serilogLoggerFactory);

            containerRegistry.RegisterInstance<IConfiguration>(configuration);

            containerRegistry.RegisterSingleton<NavigationManager>();
            containerRegistry.Register<ModulesNavigationViewModel>();

            containerRegistry.RegisterSingleton<ModuleNavigator>();
            containerRegistry.Register<IViewContext, ViewContext>();

            containerRegistry.Register<IChartHelper, ChartHelper>();

            containerRegistry.RegisterSingleton<IExchangeApiFactory, ExchangeApiFactory>();
            containerRegistry.Register<IExchangeService, ExchangeService>();

            containerRegistry.Register<ITradeViewConfigurationAccounts, TradeViewConfigurationAccountsFile>();
            containerRegistry.Register<ITradeViewConfigurationStrategy, TradeViewConfigurationStrategyFile>();
            containerRegistry.Register<ITradeViewConfigurationServer, TradeViewConfigurationServerFile>();

            containerRegistry.Register<IAccountsService, AccountsService>();
            containerRegistry.Register<IStrategyService, StrategyService>();
            containerRegistry.Register<ITradeServerService, TradeServerService>();

            containerRegistry.Register<IWpfExchangeService, WpfExchangeService>();
            containerRegistry.Register<ISymbolsCache, SymbolsCache>();
            containerRegistry.RegisterSingleton<ISymbolsCacheFactory, SymbolsCacheFactory>();
            containerRegistry.RegisterSingleton<IServerMonitorCache, ServerMonitorCache>();

            containerRegistry.RegisterSingleton<IOrderBookHelperFactory, OrderBookHelperFactory>();
            containerRegistry.RegisterSingleton<ITradeHelperFactory, TradeHelperFactory>();
            containerRegistry.RegisterSingleton<IHelperFactoryContainer, HelperFactoryContainer>();
            containerRegistry.RegisterSingleton<IChartHelper, ChartHelper>();

            containerRegistry.Register<OrdersViewModel>();
            containerRegistry.Register<AccountBalancesViewModel>();
            containerRegistry.Register<AccountViewModel>();

            containerRegistry.Register<SymbolsViewModel>();
            containerRegistry.Register<TradePanelViewModel>();

            containerRegistry.Register<IStrategyFileManager, StrategyFileManager>();
            containerRegistry.Register<ISymbolsLoader, SymbolsLoader>();

            containerRegistry.Register<IStrategyAssemblyManager, StrategyAssemblyManager>();

            containerRegistry.Register<Strategies.ViewModel.SymbolsViewModel>();
            containerRegistry.Register<Strategies.ViewModel.StrategyParametersViewModel>();

            containerRegistry.RegisterSingleton<IHttpClientManager, HttpClientManager>();
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            if(regionAdapterMappings == null)
            {
                throw new ArgumentNullException(nameof(regionAdapterMappings));
            }

            var regionBehaviorFactory = Container.Resolve<IRegionBehaviorFactory>();

            regionAdapterMappings.RegisterMapping(typeof(DockingManager), new DockingManagerRegionAdapter(regionBehaviorFactory));
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<ShellWindow>();
        }

        protected override void InitializeShell(Window shell)
        {
            if (shell == null)
            {
                throw new ArgumentNullException(nameof(shell));
            }

            var modulesNavigationViewModel = Container.Resolve<ModulesNavigationViewModel>();
            ((ShellWindow)shell).DataContext = modulesNavigationViewModel;

            Current.MainWindow = shell;
            Current.MainWindow.WindowState = WindowState.Maximized;
            Current.MainWindow.Show();

            Log.Information("*********************************************");
            Log.Information("*********************************************");
            Log.Information("Development In Progress - Wpf Market View Host");
            Log.Information("Copyright © Grant Colley 2026");
            Log.Information("Start Trade View");

            Log.Information("Shell VM modules count: {Count}", modulesNavigationViewModel.NavigationPanelItems?.Count);
        }

        private async Task SubscribeAssetsAsync()
        {
            try
            {
                var symbolsCacheFactory = Container.Resolve<ISymbolsCacheFactory>();
                await symbolsCacheFactory.SubscribeAccountsAssets().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "SubscribeAccountsAssets failed");
            }
        }

        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;
            Log.Error(ex, "Unhandled exception");
            Log.CloseAndFlush();
        }
    }
}
