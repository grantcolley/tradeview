//-----------------------------------------------------------------------
// <copyright file="NavigationManager.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentInProgress.TradeView.Wpf.Host.View;
using DevelopmentInProgress.TradeView.Wpf.Host.ViewModel;
using Prism.Regions;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Navigation
{
    /// <summary>
    /// Manages navigating views to regions using Prism. 
    /// </summary>
    public class NavigationManager
    {
        private readonly object lockNavigationList;
        private readonly IRegionManager regionManager;
        private readonly Dictionary<string, NavigationSettings> navigationSettingsList;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationManager"/> class.
        /// </summary>
        /// <param name="regionManager">The Prism region manager.</param>
        public NavigationManager(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            navigationSettingsList = new Dictionary<string, NavigationSettings>();
            lockNavigationList = new object();
        }

        /// <summary>
        /// Build up a Uri string based on the <see cref="NavigationSettings"/> argument,
        /// assign it a navigation id and store as a key value pair in the navigation settings list. 
        /// Then navigate to that view using the navigation id.
        /// </summary>
        /// <param name="navigationSettings">
        /// <see cref="NavigationSettings"/> contains information about the target view 
        /// such as the view type, view title, parameters and navigation history.
        /// </param>
        public void NavigateDocumentRegion(NavigationSettings navigationSettings)
        {
            if (String.IsNullOrEmpty(navigationSettings.View))
            {
                throw new Exception("Navigation Manager Exception : Target view not specified.");
            }

            var query = new NavigationParameters();
            query.Add("Title", navigationSettings.Title ?? navigationSettings.View);
            query.Add("Navigation", navigationSettings.NavigationHistory ?? String.Empty);

            string partialUri = navigationSettings.View + query.ToString();
            navigationSettings.PartialUri = partialUri;
            var navigationSettingsClone = (NavigationSettings)navigationSettings.Clone();
            string navigationId = String.Empty;
            lock (lockNavigationList)
            {
                var existingNavigationSetting = navigationSettingsList.Values.FirstOrDefault(
                    ns => ns.PartialUri.Equals(partialUri) 
                        && (ns.Data == null || ns.Data.Equals(navigationSettings.Data)));
                if (existingNavigationSetting != null)
                {
                    navigationId = existingNavigationSetting.NavigationId;
                }
                else
                {
                    navigationId = GetNewNavigationSettingsListKey();
                    query.Add("NavigationId", navigationId);
                    var viewUri = navigationSettings.View + query.ToString();
                    navigationSettingsClone.NavigationId = navigationId;
                    navigationSettingsClone.ViewUri = viewUri;
                    navigationSettingsList.Add(navigationId, navigationSettingsClone);
                }
            }

            NavigateDocumentRegion(navigationId); 
        }

        /// <summary>
        /// Return the next available key for the NavigationSettingsList 
        /// dictionary by getting the maximum key value and incrementing it by one.
        /// </summary>
        /// <returns>The next available key.</returns>
        private string GetNewNavigationSettingsListKey()
        {
            int maxKey = 0;
            foreach (string key in navigationSettingsList.Keys)
            {
                int iKey;
                if (Int32.TryParse(key, out iKey))
                {
                    if (iKey>maxKey)
                    {
                        maxKey = iKey;
                    }
                }
            }

            maxKey++;
            return maxKey.ToString();
        }

        /// <summary>
        /// Navigate to the DocumentRegion (document tab) passing in the view Uri.
        /// The view is obtained from the navigation list using the navigation id.
        /// </summary>
        /// <param name="navigationId">The navigation id of the Uri to navigate to.</param>
        public void NavigateDocumentRegion(string navigationId)
        {
            NavigationSettings navigationSettings;
            if (navigationSettingsList.TryGetValue(navigationId, out navigationSettings))
            {
                NavigateRegion(navigationSettings.ViewUri, "DocumentRegion");
                return;
            }

            var message = String.Format("The navigation list does not contain a Uri for navigation id {0}.", navigationId);
            throw new Exception(message);
        }

        /// <summary>
        /// Navigate to the NavigationRegion (module navigation) passing in the view Uri. 
        /// </summary>
        /// <param name="uri">The view to navigate to.</param>
        public void NavigateNavigationRegion(string uri)
        {
            NavigateRegion(uri, "NavigationRegion");
        }

        /// <summary>
        /// Uses Prism's region manager to navigate to the specified view at the specified region.
        /// </summary>
        /// <param name="uri">The view to navigate to.</param>
        /// <param name="regionName">The specified region.</param>
        public void NavigateRegion(string uri, string regionName)
        {
            regionManager.RequestNavigate(regionName,
                new Uri(uri, UriKind.Relative),
                NavigationCompleted);
        }

        /// <summary>
        /// Removes the navigation settings from the navigation 
        /// settings list of the specified navigation id.
        /// </summary>
        /// <param name="navigationId">The navigation id of the navigation settings to remove.</param>
        public void CloseDocument(string navigationId)
        {
            if (String.IsNullOrEmpty(navigationId))
            {
                return;
            }

            lock (lockNavigationList)
            {
                if (navigationSettingsList.ContainsKey(navigationId))
                {
                    navigationSettingsList.Remove(navigationId);
                }
            }
        }

        /// <summary>
        /// Gets the view model for the given navigation id.
        /// </summary>
        /// <param name="navigationId">Identifies which view model to get.</param>
        /// <returns>The <see cref="DocumentViewModel"/> for the specified navigation id.</returns>
        public DocumentViewModel GetViewModel(string navigationId)
        {
            var navigationSettings = navigationSettingsList.FirstOrDefault(n => n.Value.NavigationId.Equals(navigationId));
            return navigationSettings.Value.DocumentView.ViewModel;
        }

        /// <summary>
        /// Gets all the view models for the specified module.
        /// </summary>
        /// <param name="moduleName">The module to which the view models belong.</param>
        /// <returns>A list of <see cref="DocumentViewModel"/> for the specified module.</returns>
        public List<DocumentViewModel> GetViewModels(string moduleName)
        {
            var documentViewModels = from views
                                in navigationSettingsList
                                where views.Value.DocumentView.ModuleName.Equals(moduleName)
                                select views.Value.DocumentView.ViewModel;
            return documentViewModels.ToList();
        }

        /// <summary>
        /// Gets all view models for all modules.
        /// </summary>
        /// <returns>A list of <see cref="DocumentViewModel"/> for all modules.</returns>
        public List<DocumentViewModel> GetAllViewModels()
        {
            var documentViewModels = from views
                                in navigationSettingsList
                                select views.Value.DocumentView.ViewModel;
            return documentViewModels.ToList();
        }

        /// <summary>
        /// The navigation callback gets the view and stores a reference to it in the
        /// navigation settings. It also gets the data paremeter and passes it to the
        /// view model's by calling the Publish method.
        /// </summary>
        /// <param name="navigationResult">The navigation result.</param>
        private void NavigationCompleted(NavigationResult navigationResult)
        {
            if (navigationResult.Context.NavigationService.Region.Name.Equals("DocumentRegion"))
            {
                if (navigationResult.Result.HasValue
                    && !navigationResult.Result.Value)
                {
                    // Navigation has been cancelled.
                    return;
                }

                var query = navigationResult.Context.Parameters;
                var navigationId = query["NavigationId"].ToString();

                NavigationSettings navigationSettings;
                if (navigationSettingsList.TryGetValue(navigationId, out navigationSettings))
                {
                    object data = navigationSettings.Data;
                    var view = navigationResult.Context.NavigationService.Region.Views.FirstOrDefault(
                        v => (((DocumentViewBase)v).ViewModel.NavigationId.Equals(navigationId)));
                    var documentView = (DocumentViewBase)view;
                    navigationSettings.DocumentView = documentView;
                    documentView.ViewModel.PublishData(data);
                    return;
                }

                var message = String.Format("The navigation list does not contain a Uri for navigation id {0}.", navigationId);
                throw new Exception(message);
            }
        }
    }
}
