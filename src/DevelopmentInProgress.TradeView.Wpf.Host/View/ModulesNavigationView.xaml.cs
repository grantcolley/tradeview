//-----------------------------------------------------------------------
// <copyright file="ModulesNavigationView.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using DevelopmentInProgress.TradeView.Wpf.Controls.NavigationPanel;
using DevelopmentInProgress.TradeView.Wpf.Host.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.TradeView.Wpf.Host.View
{
    /// <summary>
    /// Interaction logic for ModulesNavigationView.xaml
    /// </summary>
    public partial class ModulesNavigationView : UserControl
    {
        private readonly NavigationManager navigationManager;
        private readonly Dictionary<string, NavigationSettings> navigationSettingsList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModulesNavigationView"/> class. 
        /// </summary>
        /// <param name="navigationManager">The navigation manager.</param>
        public ModulesNavigationView(NavigationManager navigationManager)
        {
            navigationSettingsList = new Dictionary<string, NavigationSettings>();
            this.navigationManager = navigationManager;

            InitializeComponent();

            navigationPanel.ItemSelected += SelectedModuleListItem;
        }

        /// <summary>
        /// Raises an event notifying the <see cref="DockingManagerBehavior"/> 
        /// which module has been selected. 
        /// </summary>
        public static event EventHandler<ModuleEventArgs> ModuleSelected;

        /// <summary>
        /// Adds a new module to the navigation view. Called by the <see cref="ModuleNavigator"/>.
        /// </summary>
        /// <param name="moduleSettings">Module settings.</param>
        public void AddModule(ModuleSettings moduleSettings)
        {
            var navigationPanelItem = new NavigationPanelItem();
            navigationPanelItem.NavigationPanelItemName = moduleSettings.ModuleName;
            navigationPanelItem.ImageLocation = moduleSettings.ModuleImagePath;

            foreach (ModuleGroup moduleGroup in moduleSettings.ModuleGroups)
            {
                var navigationList = new NavigationList {NavigationListName = moduleGroup.ModuleGroupName};

                foreach (ModuleGroupItem moduleGroupItem in moduleGroup.ModuleGroupItems)
                {
                    var navigationListItems = new NavigationListItem
                    {
                        ItemName = moduleGroupItem.ModuleGroupItemName,
                        ImageLocation = moduleGroupItem.ModuleGroupItemImagePath
                    };

                    navigationListItems.ItemClicked += GroupListItemItemClicked;
                    navigationList.NavigationListItems.Add(navigationListItems);

                    var navigationSettings = new NavigationSettings
                    {
                        Title = moduleGroupItem.TargetViewTitle,
                        View = moduleGroupItem.TargetView
                    };

                    string navigationKey = String.Format("{0}.{1}.{2}",
                        navigationPanelItem.NavigationPanelItemName,
                        navigationList.NavigationListName,
                        navigationListItems.ItemName);

                    navigationListItems.Tag = navigationKey;
                    navigationSettingsList.Add(navigationKey, navigationSettings);
                }

                navigationPanelItem.NavigationList.Add(navigationList);
            }

            navigationPanel.NavigationPanelItems.Add(navigationPanelItem);
        }

        /// <summary>
        /// Adds a new item to the navigation panel given the module (navigationPanelItemName) and module group ().
        /// Note, if either module or module group does not exist then an exception is thrown.
        /// </summary>
        /// <param name="navigationPanelItemName"></param>
        /// <param name="navigationListName"></param>
        /// <param name="moduleGroupItem"></param>
        public void AddNavigationListItem(string navigationPanelItemName, string navigationListName, ModuleGroupItem moduleGroupItem)
        {
            var navigationPanelItem = navigationPanel.NavigationPanelItems.FirstOrDefault(
                npi => npi.NavigationPanelItemName.Equals(navigationPanelItemName));

            var navigationList = navigationPanelItem.NavigationList.FirstOrDefault(
                nl => nl.NavigationListName.Equals(navigationListName));
            
            var navigationListItems = new NavigationListItem
            {
                ItemName = moduleGroupItem.ModuleGroupItemName,
                ImageLocation = moduleGroupItem.ModuleGroupItemImagePath
            };

            navigationListItems.ItemClicked += GroupListItemItemClicked;
            navigationList.NavigationListItems.Add(navigationListItems);

            var navigationSettings = new NavigationSettings
            {
                Title = moduleGroupItem.TargetViewTitle,
                View = moduleGroupItem.TargetView
            };

            string navigationKey = String.Format("{0}.{1}.{2}",
                navigationPanelItem.NavigationPanelItemName,
                navigationList.NavigationListName,
                navigationListItems.ItemName);

            navigationListItems.Tag = navigationKey;
            navigationSettingsList.Add(navigationKey, navigationSettings);
        }

        /// <summary>
        /// Removes an item from the navigation panel given the module (navigationPanelItemName) and module group (navigationListName).
        /// Note, if either module or module group does not exist then an exception is thrown.
        /// </summary>
        /// <param name="navigationPanelItemName"></param>
        /// <param name="navigationListName"></param>
        /// <param name="moduleGroupItemName"></param>
        public void RemoveNavigationListItem(string navigationPanelItemName, string navigationListName, string moduleGroupItemName)
        {
            var navigationPanelItem = navigationPanel.NavigationPanelItems.Single(
                npi => npi.NavigationPanelItemName.Equals(navigationPanelItemName));

            var navigationList = navigationPanelItem.NavigationList.Single(
                nl => nl.NavigationListName.Equals(navigationListName));

            var navigationListItems = navigationList.NavigationListItems.Single(nli => nli.ItemName.Equals(moduleGroupItemName));

            navigationListItems.ItemClicked -= GroupListItemItemClicked;
            navigationList.NavigationListItems.Remove(navigationListItems);

            string navigationKey = String.Format("{0}.{1}.{2}",
                navigationPanelItem.NavigationPanelItemName,
                navigationList.NavigationListName,
                navigationListItems.ItemName);

            navigationSettingsList.Remove(navigationKey);
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
            if (navigationSettingsList.TryGetValue(navigationKey, out navigationSettings))
            {
                navigationManager.NavigateDocumentRegion(navigationSettings);
            }
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
