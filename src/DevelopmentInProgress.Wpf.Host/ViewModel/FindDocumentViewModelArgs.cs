//-----------------------------------------------------------------------
// <copyright file="FindDocumentViewModel.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.Host.ViewModel
{
    /// <summary>
    /// Settings for getting view models within the same module or across all modules.
    /// A single view model can be fetched by navigation id, multiple view models can be 
    /// fetched by module name, and if neither navigation id or module name is provided then
    /// all document view models will be returned for all views. 
    /// </summary>
    public class FindDocumentViewModel
    {
        /// <summary>
        /// Gets or sets the navigation id to identify a view model to fetch.
        /// </summary>
        public string NavigationId { get; set; }

        /// <summary>
        /// Gets or sets the module name for fetching all its view models.
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Gets or sets a single view model. This is populated 
        /// when one view model is returned using the navigation id.
        /// </summary>
        public DocumentViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets or sets a list of view models. This is populated 
        /// when multiple view models are returned by module name 
        /// or when all view models across all modules are selected.
        /// </summary>
        public List<DocumentViewModel> ViewModels { get; set; }
    }
}
