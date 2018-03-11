//-----------------------------------------------------------------------
// <copyright file="IContext.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Unity;

namespace DevelopmentInProgress.Wpf.Host.Context
{
    /// <summary>
    /// Interface for the absrtact <see cref="Context"/> class inherited
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
