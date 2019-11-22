//-----------------------------------------------------------------------
// <copyright file="Context.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using Microsoft.Practices.Unity;
using Prism.Logging;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Context
{
    /// <summary>
    /// This abstract class is intended to be implemented by the
    /// <see cref="ViewContext"/> and <see cref="ViewModelContext"/>
    /// classes and provides access to the unity container 
    /// and logger facade for the given context.
    /// </summary>
    public abstract class Context : IContext
    {
        private readonly IUnityContainer unityContainer;
        private readonly ILoggerFacade logger;

        /// <summary>
        /// Initializes a new instance of the Context class.
        /// </summary>
        /// <param name="unityContainer">An instance of <see cref="IUnityContainer"/>.</param>
        /// <param name="logger">An instance of <see cref="ILoggerFacade"/>.</param>
        protected Context(IUnityContainer unityContainer, ILoggerFacade logger)
        {
            this.unityContainer = unityContainer;
            this.logger = logger;
        }

        /// <summary>
        /// An instance of <see cref="ILoggerFacade"/>.
        /// </summary>
        public ILoggerFacade Logger { get { return logger; } }

        /// <summary>
        /// An instance of <see cref="IUnityContainer"/>.
        /// </summary>
        public IUnityContainer UnityContainer { get { return unityContainer; } }
    }
}
