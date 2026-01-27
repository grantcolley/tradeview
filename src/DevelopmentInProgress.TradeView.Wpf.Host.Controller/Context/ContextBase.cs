//-----------------------------------------------------------------------
// <copyright file="Context.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

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
    public abstract class ContextBase : IContext
    {
        private readonly IUnityContainer unityContainer;
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// Initializes a new instance of the Context class.
        /// </summary>
        /// <param name="unityContainer">An instance of <see cref="IUnityContainer"/>.</param>
        /// <param name="logger">An instance of <see cref="ILoggerFactory"/>.</param>
        protected ContextBase(IUnityContainer unityContainer, ILoggerFactory loggerFactory)
        {
            this.unityContainer = unityContainer;
            this.loggerFactory = loggerFactory;
        }

        /// <summary>
        /// An instance of <see cref="ILogger"/>.
        /// </summary>
        public ILoggerFactory LoggerFactory { get { return loggerFactory; } }

        /// <summary>
        /// An instance of <see cref="IUnityContainer"/>.
        /// </summary>
        public IUnityContainer UnityContainer { get { return unityContainer; } }

        /// <summary>
        /// An instance of <see cref="ILogger"/> for the given context type."/>
        /// </summary>
        public ILogger Logger => CreateLogger(GetType());

        protected ILogger CreateLogger(Type categoryType) => loggerFactory.CreateLogger(categoryType.FullName ?? categoryType.Name);
    }
}
