//-----------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using DevelopmentInProgress.Wpf.Host.RegionAdapters;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using Serilog;
using Serilog.Core;
using Xceed.Wpf.AvalonDock;

namespace DevelopmentInProgress.Wpf.Host
{
    /// <summary>
    /// The Bootstrapper class is responsible for initializing an application using the Prism library.
    /// </summary>
    public class Bootstrapper : UnityBootstrapper
    {
        private Logger logger;

        public Bootstrapper()
        {
          logger = new LoggerConfiguration()
                .WriteTo.File("DevelopmentInProgress.Wpf.MarketView-.log", rollingInterval:RollingInterval.Day)
                .CreateLogger();
        }

        /// <summary>
        /// Create and returns an object that implements the Microsoft.Practices.Prism.Logging.ILoggerFacade.
        /// </summary>
        /// <returns>A new instance of the logger.</returns>
        protected override ILoggerFacade CreateLogger()
        {
            return new LoggerFacade.LoggerFacade(logger);
        }

        /// <summary>
        /// Load, configure and return the module catalog at startup to ensure  
        /// any module dependencies are available before the module is used.
        /// </summary>
        /// <returns>The module catalog.</returns>
        protected override IModuleCatalog CreateModuleCatalog()
        {
            using (Stream xamlStream = File.OpenRead("Configuration/ModuleCatalog.xaml"))
            {
                var moduleCatalog = Prism.Modularity.ModuleCatalog.CreateFromXaml(xamlStream);
                return moduleCatalog;
            }
        }

        /// <summary>
        /// Load Unity configurations into the container. The unity configuration file declares  
        /// and registers services (objects) that will be available via the service locator. 
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // So Unity can resolve ILogger
            Container.RegisterType<ILogger>(new ContainerControlledLifetimeManager(), new InjectionFactory((ctr, type, name) => logger));   
            
            var files = from f in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Configuration"))
                        where f.ToUpper().EndsWith("UNITY.CONFIG") select f;

            foreach (string fileName in files)
            {
                var unityMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = fileName
                };

                var unityConfig = ConfigurationManager.OpenMappedExeConfiguration(unityMap, ConfigurationUserLevel.None);
                var unityConfigSection = (UnityConfigurationSection)unityConfig.GetSection("unity");
                unityConfigSection.Configure(Container);
            }
        }

        /// <summary>
        /// Map the custom region adaptors. This enables controls as prism regions.
        /// </summary>
        /// <returns>The region adapter mappings used by prism.</returns>
        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var mappings = base.ConfigureRegionAdapterMappings();
            mappings.RegisterMapping(typeof(DockingManager), new DockingManagerRegionAdapter(ServiceLocator.Current.GetInstance<IRegionBehaviorFactory>()));
            return mappings;
        } 

        /// <summary>
        /// Override the abstract CreateShell() method to return the Shell (main window).
        /// </summary>
        /// <returns>The <see cref="Shell"/> that is the main window.</returns>
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        /// <summary>
        /// Set the <see cref="Shell"/> as the main window and show it.
        /// </summary>
        protected override void InitializeShell()
        {
            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }
    }
}