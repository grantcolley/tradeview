//-----------------------------------------------------------------------
// <copyright file="ModuleBase.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using DevelopmentInProgress.TradeView.Wpf.Host.Navigation;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Module
{
    /// <summary>
    /// Base class for Prism modules that implements Prism's IModule interface.
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        /// <summary>
        /// Initializes a new instance of the ModuleBase class.
        /// </summary>
        /// <param name="moduleNavigator">An instance of the module navigator.</param>
        /// <param name="logger">An instance of the logger.</param>
        protected ModuleBase(ModuleNavigator moduleNavigator, ILoggerFacade logger)
        {
            ModuleNavigator = moduleNavigator;
            Logger = logger;
        }

        /// <summary>
        /// Used to resgister types with the container that will be used by your application.
        /// </summary>
        /// <param name="containerRegistry">The container registry.</param>
        public abstract void RegisterTypes(IContainerRegistry containerRegistry);

        /// <summary>
        /// Notifies the module that it has been initialised.
        /// </summary>
        /// <param name="containerProvider">The container provider.</param>
        public abstract void OnInitialized(IContainerProvider containerProvider);

        /// <summary>
        /// Gets an instance of the module manager.
        /// </summary>
        public ModuleNavigator ModuleNavigator { get; private set; }

        /// <summary>
        /// Gets an instance of the logger.
        /// </summary>
        public ILoggerFacade Logger { get; private set; }
    }
}
