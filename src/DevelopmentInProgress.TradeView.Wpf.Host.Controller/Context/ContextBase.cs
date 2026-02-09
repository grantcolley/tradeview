//-----------------------------------------------------------------------
// <copyright file="Context.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using DevelopmentInProgress.TradeView.Wpf.Controls.Logging;
using Microsoft.Extensions.Logging;
using System;
using Unity;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context
{
    /// <summary>
    /// This abstract class is intended to be implemented by the
    /// <see cref="ViewContext"/> and <see cref="ViewModelContext"/>
    /// classes and provides access to the unity container 
    /// and logger facade for the given context.
    /// </summary>
    public abstract class ContextBase : LoggingBase, IContext
    {
        private readonly IUnityContainer unityContainer;

        /// <summary>
        /// Initializes a new instance of the Context class.
        /// </summary>
        /// <param name="unityContainer">An instance of <see cref="IUnityContainer"/>.</param>
        /// <param name="logger">An instance of <see cref="ILoggerFactory"/>.</param>
        protected ContextBase(IUnityContainer unityContainer, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.unityContainer = unityContainer;
        }

        /// <summary>
        /// An instance of <see cref="IUnityContainer"/>.
        /// </summary>
        public IUnityContainer UnityContainer { get { return unityContainer; } }
    }
}
