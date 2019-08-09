//-----------------------------------------------------------------------
// <copyright file="ModuleBase.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using DevelopmentInProgress.Wpf.Host.Navigation;
using Microsoft.Practices.Unity;
using Prism.Logging;
using Prism.Modularity;

namespace DevelopmentInProgress.Wpf.Host.Module
{
    /// <summary>
    /// Base class for Prism modules that implements Prism's IModule interface.
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        /// <summary>
        /// Initializes a new instance of the ModuleBase class.
        /// </summary>
        /// <param name="container">An instance of the unity container.</param>
        /// <param name="moduleNavigator">An instance of the module navigator.</param>
        /// <param name="logger">An instance of the logger.</param>
        protected ModuleBase(IUnityContainer container, ModuleNavigator moduleNavigator, ILoggerFacade logger)
        {
            Container = container;
            ModuleNavigator = moduleNavigator;
            Logger = logger;
        }

        /// <summary>
        /// The modules manager calls the Initialize method when it instantiates the module.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Gets an instance of the Unity container.
        /// </summary>
        public IUnityContainer Container { get; private set; }

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
