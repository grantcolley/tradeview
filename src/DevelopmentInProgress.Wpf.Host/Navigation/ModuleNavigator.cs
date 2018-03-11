//-----------------------------------------------------------------------
// <copyright file="ModuleNavigator.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using DevelopmentInProgress.Wpf.Host.View;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace DevelopmentInProgress.Wpf.Host.Navigation
{
    /// <summary>
    /// The ModuleManager class sets up the module navigation for each
    /// module that is configured in the ModuleCatalog.xaml file. 
    /// 
    /// Module navigation is the entry point for each module enabling the
    /// user to open documents for the module via the <see cref="ModulesNavigationView"/>.
    /// 
    /// The ModuleManager class is registered in the Unity.config file
    /// as a singleton and is injected into each module's entry class 
    /// that implements Unity's <see cref="IModule"/> interface.
    /// </summary>
    public class ModuleNavigator
    {
        private readonly IUnityContainer container;
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// Instantiates a new instance of the ModuleManager class. During instantiation  
        /// the <see cref="ModulesNavigationView"/> is registered with Unity and loaded 
        /// using Prism. 
        /// </summary>
        /// <param name="container">The unity container which is used to register the <see cref="ModulesNavigationView"/> class.</param>
        /// <param name="navigationManager">Loads the <see cref="ModulesNavigationView"/> class using Prism.</param>
        public ModuleNavigator(IUnityContainer container, NavigationManager navigationManager)
        {
            this.container = container;
            this.navigationManager = navigationManager;

            //  UNITY
            //  1. When a class is resolving from Unity as System.Object types to get it to resolve to the correct
            //     type map Unity's native System.Object resolution to the correct type for the class requested.
            // 
            //  2. Include an instance of the ContainerControlledLifetimeManager class in the parameters
            //     to the RegisterType method to instruct the container to register a singleton mapping.
            container.RegisterType<object, ModulesNavigationView>(typeof(ModulesNavigationView).Name,
                new ContainerControlledLifetimeManager());

            navigationManager.NavigateNavigationRegion(typeof(ModulesNavigationView).Name);
        }

        /// <summary>
        /// Adds module navigation for a module to the <see cref="ModulesNavigationView"/>.
        /// </summary>
        /// <param name="moduleSettings">
        /// Contains the settings for a module's navigation i.e. documents that can
        /// be opened for a module from the <see cref="ModulesNavigationView"/>.
        /// </param>
        public void AddModuleNavigation(ModuleSettings moduleSettings)
        {
            //  UNITY
            //  When a class has been registered as a named registration, to resolve for that class specify the name.
            //  Here, we resolve for the name registered singleton ModulesNavigationView.
            var modulesNavigationView = container.Resolve(typeof(ModulesNavigationView), 
                typeof(ModulesNavigationView).Name) as ModulesNavigationView;

            modulesNavigationView.AddModule(moduleSettings);
        }
    }
}