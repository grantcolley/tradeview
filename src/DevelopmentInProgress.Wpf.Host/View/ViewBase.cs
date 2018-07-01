//-----------------------------------------------------------------------
// <copyright file="ViewBase.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Windows.Controls;
using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.Navigation;
using DevelopmentInProgress.Wpf.Controls.Messaging;
using Prism.Logging;

namespace DevelopmentInProgress.Wpf.Host.View
{
    /// <summary>
    /// Base abstract class to be inherited by views.
    /// </summary>
    public abstract class ViewBase : UserControl
    {
        protected readonly IViewContext ViewContext;
        protected readonly ILoggerFacade Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewBase"/> class.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        protected ViewBase(IViewContext viewContext)
        {
            ViewContext = viewContext;
            Logger = ViewContext.Logger;
        }

        /// <summary>
        /// Handles the ShowMessageBox event raised by the view model.
        /// </summary>
        /// <param name="sender">The view model.</param>
        /// <param name="e">Message box settings.</param>
        protected void ShowMessageBox(object sender, MessageBoxSettings e)
        {
            ViewContext.ModalNavigator.ShowMessageBox(e);
        }

        /// <summary>
        /// Handles the ShowModalWindow event raised by the view model.
        /// </summary>
        /// <param name="sender">The view model.</param>
        /// <param name="e">Modal settings.</param>
        protected void ShowModalWindow(object sender, ModalSettings e)
        {
            ViewContext.ModalNavigator.ShowModal(e);
        }

        /// <summary>
        /// Handles the Publish event raised by the view model to open a new document.
        /// </summary>
        /// <param name="sender">The view model.</param>
        /// <param name="e">Navigation settings.</param>
        protected void Publish(object sender, NavigationSettings e)
        {
            ViewContext.NavigationManager.NavigateDocumentRegion(e);
        }

        /// <summary>
        /// Handles the NavigateTarget event raised by the view model to navigate to an open document.
        /// </summary>
        /// <param name="sender">The view model.</param>
        /// <param name="e">Navigation target.</param>
        protected void NavigateTarget(object sender, NavigationTarget e)
        {
            ViewContext.NavigationManager.NavigateDocumentRegion(e.NavigationId);
        }
    }
}
