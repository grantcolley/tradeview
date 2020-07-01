//-----------------------------------------------------------------------
// <copyright file="ViewModelContext.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using Prism.Logging;
using System.Windows.Threading;
using Unity;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context
{
    /// <summary>
    /// This class provides context for view models.
    /// It inherits from base abstract class <see cref="Context"/>.
    /// </summary>
    public class ViewModelContext : Context, IViewModelContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelContext"/> class.
        /// </summary>
        /// <param name="unityContainer">An instance of <see cref="IUnityContainer"/>.</param>
        /// <param name="logger">An instance of <see cref="ILoggerFacade"/>.</param>
        public ViewModelContext(IUnityContainer unityContainer, ILoggerFacade logger)
            : base(unityContainer, logger)
        {
        }

        /// <summary>
        /// Gets or sets the UI Dispatcher.
        /// </summary>
        public Dispatcher UiDispatcher { get; set; }
    }
}
