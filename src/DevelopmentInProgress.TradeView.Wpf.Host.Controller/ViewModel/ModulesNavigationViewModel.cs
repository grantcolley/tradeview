using DevelopmentInProgress.TradeView.Wpf.Controls.Navigation;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Controller.ViewModel
{
    public class ModulesNavigationViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<NavigationPanelItem> navigationPanelItems;

        public ModulesNavigationViewModel()
        {
            NavigationSettingsList = new Dictionary<string, NavigationSettings>();
            navigationPanelItems = new ObservableCollection<NavigationPanelItem>();
        }

        public event EventHandler<NavigationEventArgs> RegisterNavigation;
        public event EventHandler<NavigationEventArgs> UnregisterNavigation;
        public event PropertyChangedEventHandler PropertyChanged;

        public readonly Dictionary<string, NavigationSettings> NavigationSettingsList;

        public ObservableCollection<NavigationPanelItem> NavigationPanelItems
        {
            get { return navigationPanelItems; }
            set 
            {
                if(navigationPanelItems != value)
                {
                    navigationPanelItems = value;
                    OnPropertyChanged(nameof(NavigationPanelItems));
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Adds a new module to the navigation view. Called by the <see cref="ModuleNavigator"/>.
        /// </summary>
        /// <param name="moduleSettings">Module settings.</param>
        public void AddModule(ModuleSettings moduleSettings)
        {
            if (moduleSettings == null)
            {
                throw new ArgumentNullException(nameof(moduleSettings));
            }

            var navigationPanelItem = new NavigationPanelItem
            {
                NavigationPanelItemName = moduleSettings.ModuleName,
                ImageLocation = moduleSettings.ModuleImagePath
            };

            foreach (ModuleGroup moduleGroup in moduleSettings.ModuleGroups)
            {
                var navigationList = new NavigationList { NavigationListName = moduleGroup.ModuleGroupName };

                foreach (ModuleGroupItem moduleGroupItem in moduleGroup.ModuleGroupItems)
                {
                    var navigationListItem = new NavigationListItem
                    {
                        ItemName = moduleGroupItem.ModuleGroupItemName,
                        ImageLocation = moduleGroupItem.ModuleGroupItemImagePath
                    };

                    OnRegisterNavigation(navigationListItem);

                    navigationList.NavigationListItems.Add(navigationListItem);

                    var navigationSettings = new NavigationSettings
                    {
                        Title = moduleGroupItem.TargetViewTitle,
                        View = moduleGroupItem.TargetView
                    };

                    string navigationKey = string.Format("{0}.{1}.{2}",
                        navigationPanelItem.NavigationPanelItemName,
                        navigationList.NavigationListName,
                        navigationListItem.ItemName);

                    navigationListItem.Tag = navigationKey;
                    NavigationSettingsList.Add(navigationKey, navigationSettings);
                }

                navigationPanelItem.NavigationList.Add(navigationList);
            }

            NavigationPanelItems.Add(navigationPanelItem);
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
            if (moduleGroupItem == null)
            {
                throw new ArgumentNullException(nameof(moduleGroupItem));
            }

            var navigationPanelItem = NavigationPanelItems.FirstOrDefault(
                npi => npi.NavigationPanelItemName.Equals(navigationPanelItemName, StringComparison.Ordinal));

            var navigationList = navigationPanelItem.NavigationList.FirstOrDefault(
                nl => nl.NavigationListName.Equals(navigationListName, StringComparison.Ordinal));

            var navigationListItem = new NavigationListItem
            {
                ItemName = moduleGroupItem.ModuleGroupItemName,
                ImageLocation = moduleGroupItem.ModuleGroupItemImagePath
            };

            OnRegisterNavigation(navigationListItem);

            navigationList.NavigationListItems.Add(navigationListItem);

            var navigationSettings = new NavigationSettings
            {
                Title = moduleGroupItem.TargetViewTitle,
                View = moduleGroupItem.TargetView
            };

            string navigationKey = string.Format("{0}.{1}.{2}",
                navigationPanelItem.NavigationPanelItemName,
                navigationList.NavigationListName,
                navigationListItem.ItemName);

            navigationListItem.Tag = navigationKey;
            NavigationSettingsList.Add(navigationKey, navigationSettings);
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
            var navigationPanelItem = NavigationPanelItems.Single(
                npi => npi.NavigationPanelItemName.Equals(navigationPanelItemName, StringComparison.Ordinal));

            var navigationList = navigationPanelItem.NavigationList.Single(
                nl => nl.NavigationListName.Equals(navigationListName, StringComparison.Ordinal));

            var navigationListItem = navigationList.NavigationListItems.Single(nli => nli.ItemName.Equals(moduleGroupItemName, StringComparison.Ordinal));

            OnUnregisterNavigation(navigationListItem);

            navigationList.NavigationListItems.Remove(navigationListItem);

            string navigationKey = string.Format("{0}.{1}.{2}",
                navigationPanelItem.NavigationPanelItemName,
                navigationList.NavigationListName,
                navigationListItem.ItemName);

            NavigationSettingsList.Remove(navigationKey);
        }

        private void OnRegisterNavigation(NavigationListItem navigationListItem)
        {
            var registerNavigation = RegisterNavigation;
            registerNavigation?.Invoke(this, new NavigationEventArgs(navigationListItem));
        }

        private void OnUnregisterNavigation(NavigationListItem navigationListItem)
        {
            var unregisterNavigation = UnregisterNavigation;
            unregisterNavigation?.Invoke(this, new NavigationEventArgs(navigationListItem));
        }
    }
}
