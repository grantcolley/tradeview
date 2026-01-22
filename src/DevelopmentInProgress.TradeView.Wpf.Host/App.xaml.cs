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
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using Prism.Unity;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.Windows;
using Xceed.Wpf.AvalonDock;

namespace DevelopmentInProgress.TradeView.Wpf.Host
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
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

        protected override async void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Serilog.Core.Logger logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            var loggerFactory = new SerilogLoggerFactory(Log.Logger, dispose: false);

            containerRegistry.RegisterInstance<ILoggerFactory>(loggerFactory);
            containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));

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

            var serverMonitorCache = Container.Resolve<IServerMonitorCache>();
            serverMonitorCache.StartObservingServers();

            var symbolsCacheFactory = Container.Resolve<ISymbolsCacheFactory>();
            await symbolsCacheFactory.SubscribeAccountsAssets().ConfigureAwait(false);
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
            ((ShellWindow)shell).ModulesNavigationViewModel = modulesNavigationViewModel;

            Current.MainWindow = shell;
            Current.MainWindow.WindowState = WindowState.Maximized;
            Current.MainWindow.Show();

            var logger = Container.Resolve<ILogger<App>>();
            logger.LogInformation("*********************************************");
            logger.LogInformation("*********************************************");
            logger.LogInformation("Development In Progress - Wpf Market View Host");
            logger.LogInformation("Copyright © Grant Colley 2026");
            logger.LogInformation("Start Trade View");
        }

        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            var logger = Container.Resolve<ILogger<App>>();
            logger.LogError(e.ToString());
        }
    }
}
