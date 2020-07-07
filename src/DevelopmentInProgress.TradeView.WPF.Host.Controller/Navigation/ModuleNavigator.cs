//-----------------------------------------------------------------------
// <copyright file="ModuleNavigator.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using DevelopmentInProgress.TradeView.Wpf.Host.Controller.View;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.ViewModel;
using Prism.Modularity;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Controller.Navigation
{
    /// <summary>
    /// The <see cref="ModuleNavigator"/> class sets up the module navigation for each
    /// module that is configured in the ModuleCatalog.xaml file. 
    /// 
    /// Module navigation is the entry point for each module enabling the
    /// user to open documents for the module via the <see cref="ModulesNavigationViewModel"/>.
    /// 
    /// The <see cref="ModuleNavigator"/> class is injected into each module's entry class 
    /// that implements Unity's <see cref="IModule"/> interface.
    /// </summary>
    public class ModuleNavigator
    {
        private readonly ModulesNavigationViewModel modulesNavigationViewModel;

        /// <summary>
        /// Instantiates a new instance of the <see cref="ModuleNavigator"/> class.
        /// </summary>
        /// <param name="modulesNavigationView">The <see cref="ModulesNavigationView"/> class.</param>
        public ModuleNavigator(ModulesNavigationViewModel modulesNavigationView)
        {
            this.modulesNavigationViewModel = modulesNavigationView;
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
            modulesNavigationViewModel.AddModule(moduleSettings);
        }

        public void AddNavigationListItem(string navigationPanelItemName, string navigationListName, ModuleGroupItem moduleGroupItem)
        {
            modulesNavigationViewModel.AddNavigationListItem(navigationPanelItemName, navigationListName, moduleGroupItem);
        }

        public void RemoveNavigationListItem(string navigationPanelItemName, string navigationListName, string moduleGroupItemName)
        {
            modulesNavigationViewModel.RemoveNavigationListItem(navigationPanelItemName, navigationListName, moduleGroupItemName);
        }
    }
}