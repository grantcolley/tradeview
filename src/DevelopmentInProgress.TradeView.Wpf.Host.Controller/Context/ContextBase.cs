//-----------------------------------------------------------------------
// <copyright file="Context.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
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
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the Context class.
        /// </summary>
        /// <param name="unityContainer">An instance of <see cref="IUnityContainer"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger"/>.</param>
        protected ContextBase(IUnityContainer unityContainer, ILogger logger)
        {
            this.unityContainer = unityContainer;
            this.logger = logger;
        }

        /// <summary>
        /// An instance of <see cref="ILogger"/>.
        /// </summary>
        public ILogger Logger { get { return logger; } }

        /// <summary>
        /// An instance of <see cref="IUnityContainer"/>.
        /// </summary>
        public IUnityContainer UnityContainer { get { return unityContainer; } }
    }
}
