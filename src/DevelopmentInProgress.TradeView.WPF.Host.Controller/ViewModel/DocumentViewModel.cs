//-----------------------------------------------------------------------
// <copyright file="DocumentViewModel.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Navigation;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Controller.ViewModel
{
    /// <summary>
    /// Base view model for documents.
    /// </summary>
    public abstract class DocumentViewModel : ViewModelBase, IDisposable, INavigationAware /*, IConfirmNavigationRequest, IRegionMemberLifetime */
    {
        private readonly List<NavigationTarget> navigationHistory;
        private string uriQueryString;
        private string navigationId;
        private object data;
        private ICommand navigateDocument;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentViewModel"/> class.
        /// </summary>
        /// <param name="viewModelContext">The view model context.</param>
        protected DocumentViewModel(IViewModelContext viewModelContext)
            : base(viewModelContext)
        {
            navigationHistory = new List<NavigationTarget>();
            NavigateDocument = new ViewModelCommand(OnNavigateDocument);
        }

        /// <summary>
        /// Virtual method to be implemented by the sub class notifying the document when it has become active.
        /// </summary>
        /// <param name="isActive">Indicates whether the document is the current active document in the docking manager.</param>
        public virtual void OnActiveChanged(bool isActive)
        {
        }

        /// <summary>
        /// Virtual method to be implemented by the sub class when a document is loaded.
        /// </summary>
        /// <param name="data">The parameter passed into the method.</param>
        protected virtual void OnPublished(object data)
        {
        }

        /// <summary>
        /// Virtual method to be implemented by the sub class when the document is disposing.
        /// </summary>
        protected virtual void OnDisposing()
        {
        }

        /// <summary>
        /// Raised when publishing a document. Handled in the <see cref="DocumentViewBase"/> class.
        /// </summary>
        public event EventHandler<NavigationSettings> Publish;

        /// <summary>
        /// Raised when navigating to an existing document. Handled in the <see cref="DocumentViewBase"/> class.
        /// </summary>
        public event EventHandler<NavigationTarget> NavigateTarget;

        /// <summary>
        /// Identifier used for navigating to the document after it has been created.
        /// </summary>
        public string NavigationId 
        {
            get { return navigationId; }
            private set { navigationId = value; }
        }

        /// <summary>
        /// The command which executes the OnNavigateDocument method which raises the <see cref="NavigateTarget"/> event.
        /// </summary>
        public ICommand NavigateDocument
        {
            get { return navigateDocument; }
            set { navigateDocument = value; }
        }

        /// <summary>
        /// A list of documents showing the history of documents opened leading to the 
        /// current one. Also provides a means to navigate back to a document in the list.
        /// </summary>
        public List<NavigationTarget> NavigationHistory
        {
            get { return navigationHistory; }
        }

        /// <summary>
        /// The parameter passed in to the current document.
        /// </summary>
        protected object Data
        {
            get { return data; }
            private set { data = value; }
        }

        /// <summary>
        /// Called by the <see cref="NavigationManager"/> in order to
        /// pass in the document parameters when the form is first loaded.
        /// </summary>
        /// <param name="args">The document parameters.</param>
        public void PublishData(object args)
        {
            Data = args;
            DataPublished();
        }

        /// <summary>
        /// Called by the <see cref="ViewModelBase"/> DataPublished method and
        /// in turn calls the abstract OnPublished method implemented by the 
        /// specialized ViewModel, passing the data, when the document gets loaded.
        /// </summary>
        protected override void OnPublished()
        {
            OnPublished(Data);
        }

        /// <summary>
        /// Called to publish a new document. Calls the <see cref="Publish"/> 
        /// event which is handled on the <see cref="ViewBase"/>.
        /// </summary>
        /// <param name="navigationSettings">Navigation settings for the new document.</param>
        protected void PublishDocument(NavigationSettings navigationSettings)
        {
            var publish = Publish;
            if (publish != null)
            {
                var navigationTarget = new NavigationTarget(navigationId, Title);
                if (navigationHistory.Count > 0)
                {
                    navigationTarget.AppendNavigationHistory(navigationHistory.Select(t => t.Target).ToArray());
                }

                navigationSettings.NavigationHistory = navigationTarget.NavigationHistory;
                publish(this, navigationSettings);
            }
        }

        /// <summary>
        /// Raised the <see cref="NavigationTarget"/> event to navigate to an existing document.
        /// </summary>
        /// <param name="target">The title of the target document to navigate to.</param>
        private void OnNavigateDocument(object target)
        {
            var navigateTarget = NavigateTarget;
            if (navigateTarget != null)
            {
                var navigationTarget = navigationHistory.First(n => n.Title.Equals(target));
                navigateTarget(this, navigationTarget);
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                OnDisposing();

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~DocumentViewModel()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: only call the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Implement INavigationAware

        /// <summary>
        /// Handled by the <see cref="DocumentViewBase"/> to set the 
        /// document title and make it the active document in the 
        /// docking manager.
        /// </summary>
        public event EventHandler Activate;

        /// <summary>
        /// Handles a Prism event to determine whether the document is the target.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        /// <returns>True if it is the target, else returns false.</returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (!String.IsNullOrEmpty(uriQueryString)
                && uriQueryString.Equals(navigationContext.Uri.OriginalString))
            {
                RaiseActivation();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Raises the <see cref="Activate"/> event handled by 
        /// the <see cref="DocumentViewBase"/> to set the current 
        /// document as the active document in the docking manager. 
        /// </summary>
        private void RaiseActivation()
        {
            var activateHandler = Activate;
            if (activateHandler != null)
            {
                activateHandler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Indentifies the document that created the current document.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        /// <summary>
        /// Called by Prism when navigating to the current document. 
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (!String.IsNullOrEmpty(uriQueryString)
                && uriQueryString.Equals(navigationContext.Uri.OriginalString))
            {
                return;
            }

            uriQueryString = navigationContext.Uri.OriginalString;

            foreach (KeyValuePair<string, object> parameter in navigationContext.Parameters)
            {
                if (parameter.Key.Equals("Title"))
                {
                    Title = parameter.Value?.ToString();
                    continue;
                }

                if (parameter.Key.Equals("NavigationId"))
                {
                    NavigationId = parameter.Value?.ToString();
                    continue;
                }

                if (parameter.Key.Equals("Navigation"))
                {
                    string[] history = NavigationTarget.GetNavigationHistory(parameter.Value?.ToString());
                    foreach (string target in history)
                    {
                        if (!String.IsNullOrEmpty(target))
                        {
                            navigationHistory.Add(new NavigationTarget(target));
                        }
                    }
                }
            }

            RaiseActivation();
        }

        #endregion

        #region Implement IRegionMemberLifetime

        // DO NOT IMPLEMENT IRegionMemberLifetime
        // This seems to raise an exception when explicitly removing the view from the
        // view manager which leads to a catch-22 because without removing the view 
        // another one cant be added as the region manager thinks it still exists.
        // public bool KeepAlive { get { return false; } }

        #endregion

        #region Implement IConfirmNavigationRequest

        /* NOT IMPLEMENTING AS WILL LEAD TO INCONSISTENT BEHAVIOUR
         
        This currently only works when navigating away from one document by opening another via Prism.
        The problem is, there currently isn't a mechanism to get confirmation before closing the document or changing the  
        focus to another open documents or before setting the visibility to false when selecting another module in the navigation bar.

        /// <summary>
        /// Handles the navigation callback to confirm whether to 
        /// proceed to navigate away from the current document. The 
        /// callback calls the abstract CanNavigateAway method implemeted
        /// by the subclass to confirm the navigation request.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        /// <param name="continuationCallback">The continuation callback.</param>
        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(CanNavigateAway());
        }

        */

        #endregion
    }
}
