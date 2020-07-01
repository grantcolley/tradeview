//-----------------------------------------------------------------------
// <copyright file="ModalViewModel.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Navigation;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Controller.ViewModel
{
    /// <summary>
    /// Base view model for modal windows. Inherits <see cref="ViewModelBase"/>.
    /// </summary>
    public abstract class ModalViewModel : ViewModelBase
    {
        private Dictionary<string, object> parameters;

        /// <summary>
        /// Initialises a new instance of the <see cref="ModalViewModel"/> class.
        /// </summary>
        /// <param name="viewModelContext">The <see cref="ViewModelContext"/>.</param>
        protected ModalViewModel(IViewModelContext viewModelContext)
            : base(viewModelContext)
        {
        }

        /// <summary>
        /// Output of the modal window which can be accessed from 
        /// the calling code when the modal window is closed.
        /// </summary>
        public object Output { get; set; }

        /// <summary>
        /// Called by the <see cref="ModalNavigator"/> to pass a collection of parameters.
        /// </summary>
        /// <param name="param">A dictionary of parameters.</param>
        public void Publish(Dictionary<string, object> param)
        {
            parameters = param;
            DataPublished();
            OnPropertyChanged(String.Empty);
        }

        /// <summary>
        /// To be overriden by the the sub class.
        /// </summary>
        /// <param name="param">A dictionary of parameters.</param>
        protected virtual void OnPublished(Dictionary<string, object> param)
        {
        }

        /// <summary>
        /// Implemnents the abstract OnPublished method of the <see cref="ViewModelBase"/>.
        /// Calls OnPublished to be implemented by the subclass.
        /// </summary>
        protected override void OnPublished()
        {
            OnPublished(parameters);
        }
    }
}
