//-----------------------------------------------------------------------
// <copyright file="IContext.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using Prism.Logging;
using Unity;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context
{
    /// <summary>
    /// Interface for the absrtact <see cref="ContextBase"/> class inherited
    /// by <see cref="ViewContext"/> and <see cref="ViewModelContext"/>.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// An instance of <see cref="ILoggerFacade"/>.
        /// </summary>
        ILoggerFacade Logger { get; }

        /// <summary>
        /// An instance of <see cref="IUnityContainer"/>.
        /// </summary>
        IUnityContainer UnityContainer { get; }
    }
}
