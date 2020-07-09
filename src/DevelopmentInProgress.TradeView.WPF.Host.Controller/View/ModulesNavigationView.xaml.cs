//-----------------------------------------------------------------------
// <copyright file="ModulesNavigationView.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using CommonServiceLocator;
using DevelopmentInProgress.TradeView.Wpf.Controls.NavigationPanel;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Navigation;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Controller.View
{
    /// <summary>
    /// Interaction logic for ModulesNavigationView.xaml
    /// </summary>
    public partial class ModulesNavigationView : UserControl
    {
        private readonly NavigationManager navigationManager;
        private readonly ModulesNavigationViewModel modulesNavigationViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModulesNavigationView"/> class. 
        /// </summary>
        public ModulesNavigationView()
        {
            InitializeComponent();

            modulesNavigationViewModel = ServiceLocator.Current.GetInstance<ModulesNavigationViewModel>();
            navigationManager = ServiceLocator.Current.GetInstance<NavigationManager>();

            modulesNavigationViewModel.RegisterNavigation += ModulesNavigationViewModelRegisterNavigation;
            modulesNavigationViewModel.UnregisterNavigation += ModulesNavigationViewModelUnregisterNavigation;

            DataContext = this.modulesNavigationViewModel;

            navigationPanel.ItemSelected += SelectedModuleListItem;
        }

        /// <summary>
        /// Raises an event notifying the <see cref="DockingManagerBehavior"/> 
        /// which module has been selected. 
        /// </summary>
        public static event EventHandler<ModuleEventArgs> ModuleSelected;

        private void ModulesNavigationViewModelUnregisterNavigation(object sender, NavigationEventArgs e)
        {
            e.NavigationListItem.ItemClicked -= GroupListItemItemClicked;
        }

        private void ModulesNavigationViewModelRegisterNavigation(object sender, NavigationEventArgs e)
        {
            e.NavigationListItem.ItemClicked += GroupListItemItemClicked;
        }

        /// <summary>
        /// Opens a new document.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event arguments.</param>
        private void GroupListItemItemClicked(object sender, RoutedEventArgs e)
        {
            var navigationListItem = (NavigationListItem)e.Source;
            string navigationKey = navigationListItem.Tag.ToString();
            NavigationSettings navigationSettings;
            if (((ModulesNavigationViewModel)DataContext).NavigationSettingsList.TryGetValue(navigationKey, out navigationSettings))
            {
                navigationManager.NavigateDocumentRegion(navigationSettings);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Raises the <see cref="ModuleSelected"/> event which is handled by the <see cref="DockingManagerBehavior"/>. 
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SelectedModuleListItem(object sender, RoutedEventArgs e)
        {
            var navigationPanelItem = (NavigationPanelItem)e.OriginalSource;
            var moduleSelected = ModuleSelected;
            if (moduleSelected != null)
            {
                var modulePaneEventArgs = new ModuleEventArgs(navigationPanelItem.NavigationPanelItemName);
                moduleSelected(this, modulePaneEventArgs);
            }

            e.Handled = true;
        }
    }
}
