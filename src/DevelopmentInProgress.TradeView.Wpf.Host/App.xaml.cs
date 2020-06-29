using CommonServiceLocator;
using DevelopmentInProgress.TradeView.Wpf.Host.Logger;
using DevelopmentInProgress.TradeView.Wpf.Host.RegionAdapters;
using Microsoft.Practices.Unity.Configuration;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using Serilog;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using Xceed.Wpf.AvalonDock;

namespace DevelopmentInProgress.TradeView.Wpf.Host
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Serilog.Core.Logger logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            containerRegistry.RegisterInstance<ILogger>(logger);
            containerRegistry.RegisterSingleton<ILoggerFacade, LoggerFacade>();

            var files = from f in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Configuration"))
                        where f.ToUpper().EndsWith("UNITY.CONFIG")
                        select f;


            // configuring the container declaratively from a configuration file
            // https://www.nuget.org/packages/Unity.NetCore/

            foreach (string fileName in files)
            {
                var unityMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = fileName
                };

                var unityConfig = ConfigurationManager.OpenMappedExeConfiguration(unityMap, ConfigurationUserLevel.None);
                var unityConfigSection = (UnityConfigurationSection)unityConfig.GetSection("unity");
                unityConfigSection.Configure(Container.GetContainer());
            }
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            using (Stream xamlStream = File.OpenRead("Configuration/ModuleCatalog.xaml"))
            {
                var moduleCatalog = ModuleCatalog.CreateFromXaml(xamlStream);
                return moduleCatalog;
            }
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            regionAdapterMappings.RegisterMapping(typeof(DockingManager), new DockingManagerRegionAdapter(ServiceLocator.Current.GetInstance<IRegionBehaviorFactory>()));
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell(Window shell)
        {
            Current.MainWindow = shell;
            Current.MainWindow.WindowState = WindowState.Maximized;
            Current.MainWindow.Show();
        }
    }
}
