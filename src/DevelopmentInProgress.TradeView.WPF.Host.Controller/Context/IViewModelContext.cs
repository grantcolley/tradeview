//-----------------------------------------------------------------------
// <copyright file="IViewModelContext.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Windows.Threading;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context
{
    /// <summary>
    /// Interface for the <see cref="ViewModelContext"/> class
    /// which inherits abstract class <see cref="ContextBase"/>.
    /// </summary>
    public interface IViewModelContext : IContext
    {
        /// <summary>
        /// Gets or sets the UI Dispatcher.
        /// </summary>
        Dispatcher UiDispatcher { get; set; }
    }
}
