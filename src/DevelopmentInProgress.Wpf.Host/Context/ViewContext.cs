//-----------------------------------------------------------------------
// <copyright file="ViewContext.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using DevelopmentInProgress.Wpf.Host.Navigation;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Unity;

namespace DevelopmentInProgress.Wpf.Host.Context
{
    /// <summary>
    /// This class provides context for views, giving access to the <see cref="ModalNavigator"/> 
    /// and <see cref="NavigationManager"/> which enable it to manage model windows and documents 
    /// via prism navigation. It inherits from base abstract class <see cref="Context"/>.
    /// </summary>
    public class ViewContext : Context, IViewContext
    {
        private readonly ModalNavigator modalManager;
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewContext"/> class.
        /// </summary>
        /// <param name="unityContainer">An instance of <see cref="IUnityContainer"/>.</param>
        /// <param name="modalManager">An instance of <see cref="ModalNavigator"/>.</param>
        /// <param name="navigationManager">An instance of <see cref="NavigationManager"/>.</param>
        /// <param name="logger">An instance of <see cref="ILoggerFacade"/>.</param>
        public ViewContext(IUnityContainer unityContainer, ModalNavigator modalManager, NavigationManager navigationManager, ILoggerFacade logger)
            : base(unityContainer, logger)
        {
            this.modalManager = modalManager;
            this.navigationManager = navigationManager;
        }

        /// <summary>
        /// An instance of <see cref="ModalNavigator"/> 
        /// enabling it to manage model pop-up windows.
        /// </summary>
        public ModalNavigator ModalNavigator { get { return modalManager; } }

        /// <summary>
        /// An instance of <see cref="NavigationManager"/> enabling it to 
        /// manage model windows and to manage documents via prism navigation.
        /// </summary>
        public NavigationManager NavigationManager { get { return navigationManager; } }
    }
}
