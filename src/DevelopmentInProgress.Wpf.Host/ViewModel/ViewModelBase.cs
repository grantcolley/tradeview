//-----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.Navigation;
using DevelopmentInProgress.Wpf.Host.View;
using DevelopmentInProgress.Wpf.Controls.Messaging;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Logging;

namespace DevelopmentInProgress.Wpf.Host.ViewModel
{
    /// <summary>
    /// Base abstract class to be inherited by ViewModel's providing 
    /// implementation for common features across view models.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected readonly ILoggerFacade Logger;
        private string title;
        private bool isBusy;
        private bool isDirty;
        private bool isMessagesExpanded;
        private ObservableCollection<Message> messages;
        private ICommand refresh;
        private ICommand save;
        private ICommand clearMessageBox;

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        /// <param name="viewModelContext">The <see cref="ViewModelContext"/>.</param>
        protected ViewModelBase(IViewModelContext viewModelContext)
        {
            ViewModelContext = viewModelContext;
            Logger = ViewModelContext.Logger;
            Save = new ViewModelCommand(OnSave);
            Refresh = new ViewModelCommand(OnRefresh);
            ClearMessageBox = new ViewModelCommand(OnClearMessages);
        }

        /// <summary>
        /// Raised when the view model wants to show a message in a mesage box.
        /// Typically handled by the <see cref="DocumentViewBase"/> class.
        /// </summary>
        public event EventHandler<MessageBoxSettings> ShowMessageWindow;

        /// <summary>
        /// Raised when the view model wants to show a modal window.
        /// Typically handled by the <see cref="DocumentViewBase"/> class.
        /// </summary>
        public event EventHandler<ModalSettings> ShowModalWindow;

        /// <summary>
        /// Raised when the view model wants to get access to another view model.
        /// </summary>
        public event EventHandler<FindDocumentViewModel> GetViewModels;

        /// <summary>
        /// Raised to notify a property value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the view model context.
        /// </summary>
        public IViewModelContext ViewModelContext { get; private set; }

        #region CanNavigateAway - Not yet implemented

        /*  Do not implement until there is a mechanism to consistently check whether a document can navigate away 
            i.e. open another document via Prism, lose focus when toggling between existing documents or clicking 
            another module in the navigation bar or before closing the document.
         
        /// <summary>
        /// Determines whether the document can be closed or lose focus 
        /// i.e. providing the opportunity to check if the user wants 
        /// to perform an action first, such as saving the document.
        /// </summary>
        /// <returns>True if can navigate away, else returns false.</returns>
        protected abstract bool CanNavigateAway();

        */

        #endregion

        /// <summary>
        /// Notification to the sub class that data has been published. This is called  
        /// when the document is opened for the first time and when it is refreshed.
        /// This abstract method is implemented by <see cref="DocumentViewModel"/> and <see cref="ModalViewModel"/>
        /// which in turn implement a specialized virtual OnPublish method of their own which can be overriden by a 
        /// document or modal view model. 
        /// </summary>
        protected abstract void OnPublished();

        /// <summary>
        /// Optionally overriden by the view model to handle saving the document when clicking the save icon.
        /// </summary>
        protected virtual void SaveDocument()
        {
        }

        /// <summary>
        /// Bind the Save command to a control's Command on the view.
        /// Save is also used by the <see cref="Shell"/> when saving the 
        /// currently active document from the main menu, or saving all 
        /// visible documents from the main menu.
        /// </summary>
        public ICommand Save
        {
            get { return save; }
            set { save = value; }
        }

        /// <summary>
        /// Bind the Refresh command to a control's Command on the view.
        /// Refresh is also used by the <see cref="DocumentViewHost"/> when 
        /// refreshing the currently active document, or saving all 
        /// visible documents from the main menu of the <see cref="Shell"/>.
        /// </summary>
        public ICommand Refresh
        {
            get { return refresh; }
            set { refresh = value; }
        }

        /// <summary>
        /// Command for clearing the messages collection.
        /// </summary>
        public ICommand ClearMessageBox
        {
            get { return clearMessageBox; }
            set { clearMessageBox = value; }
        }

        /// <summary>
        /// View model title.
        /// </summary>
        public string Title
        {
            get { return title; }
            protected set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        /// <summary>
        /// Indicates whether the view model is busy processing. 
        /// Is used to set or hide a visual indicator on the view.
        /// </summary>
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    OnPropertyChanged("IsBusy");
                }
            }
        }

        /// <summary>
        /// Indicates whether the view had been modified i.e. data changed
        /// and may needs to be saved before closing or navigating away.
        /// This is set and evaluated from the sub class view model.
        /// </summary>
        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    OnPropertyChanged("IsDirty");
                }
            }
        }

        /// <summary>
        /// A collection of <see cref="Message"/>
        /// </summary>
        public ObservableCollection<Message> Messages
        {
            get { return messages; }
        }

        /// <summary>
        /// A value indicating whether there are 
        /// messages that need to be displayed. 
        /// </summary>
        public bool IsMessagesVisible
        {
            get
            {
                if (messages != null
                    && messages.Count > 0)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to expand the control that displays messages.
        /// </summary>
        public bool IsMessagesExpanded
        {
            get { return isMessagesExpanded; }
            set
            {
                isMessagesExpanded = value;
                OnPropertyChanged("IsMessagesExpanded");
            }
        }

        /// <summary>
        /// Clears the messages collection and hides the messages control.
        /// </summary>
        public void ClearMessages()
        {
            messages = null;
            OnPropertyChanged("Messages");
            OnPropertyChanged("IsMessagesVisible");
        }

        /// <summary>
        /// Set IsBusy and IsDirty indicators to false.
        /// </summary>
        protected void ResetStatus()
        {
            IsBusy = false;
            IsDirty = false;
            OnPropertyChanged("IsBusy");
            OnPropertyChanged("IsDirty");
        }

        /// <summary>
        /// Raises the <see cref="ShowMessageWindow"/> event which is handled on the view.
        /// </summary>
        /// <param name="messageBoxSettings">Details of the message to display.</param>
        /// <returns>The message result.</returns>
        protected MessageBoxResult ShowMessageBox(MessageBoxSettings messageBoxSettings)
        {
            var showMessageWindow = ShowMessageWindow;
            if (showMessageWindow != null)
            {
                showMessageWindow(this, messageBoxSettings);
            }

            return messageBoxSettings.MessageBoxResult;
        }

        /// <summary>
        /// Raises the <see cref="ShowModalWindow"/> event which is handled on the view.
        /// </summary>
        /// <param name="modalSettings">Details of the modal form to display.</param>
        protected void ShowModal(ModalSettings modalSettings)
        {
            var showModalWindow = ShowModalWindow;
            if (showModalWindow != null)
            {
                showModalWindow(this, modalSettings);
            }
        }

        /// <summary>
        /// Adds a message to the message collection and then shows it.
        /// </summary>
        /// <param name="message">The message to add.</param>
        /// <param name="appendMessage">Indicates whether the message is to be  
        /// appened to existing messages or existing messages are first cleared.</param>
        protected void ShowMessage(Message message, bool appendMessage = false)
        {
            var messagesToShow = new List<Message>();
            messagesToShow.Add(message);
            ShowMessages(messagesToShow, appendMessage);
        }

        /// <summary>
        /// Shows the list of messages.
        /// </summary>
        /// <param name="messagesToShow">The messages to show.</param>
        /// <param name="appendMessage">Indicates whether the message is to be  
        /// appened to existing messages or existing messages are first cleared.</param>
        protected void ShowMessages(List<Message> messagesToShow, bool appendMessage = false)
        {
            Action<List<Message>, bool> action = (msgs, append) =>
            {
                if (messages == null)
                {
                    messages = new ObservableCollection<Message>();
                }

                msgs.ForEach(
                    m => Logger.Log(m.Text, ConvertMessageTypeToLogCategory(m.MessageType), Priority.None));

                if (append)
                {
                    msgs.ForEach(m => this.messages.Insert(0, m));
                }
                else
                {
                    messages.Clear();
                    messages.AddRange(msgs);
                }

                IsMessagesExpanded = true;
                OnPropertyChanged("Messages");
                OnPropertyChanged("IsMessagesVisible");
            };

            ViewModelContext.UiDispatcher.Invoke(action, new object[] { messagesToShow, appendMessage });
        }

        /// <summary>
        /// Raised the <see cref="PropertyChanged"/> event 
        /// which notifies the view to update the control 
        /// binding to the specified property. When the property
        /// name is not specified, all controls are updated.
        /// </summary>
        /// <param name="propertyName">The name of the property to notify. If empty, all controls on the view are updated.</param>
        /// <param name="isDirty">Optionally sets the is dirty flag.</param>
        protected void OnPropertyChanged(string propertyName, bool isDirty = false)
        {
            if (isDirty)
            {
                IsDirty = true;
            }

            var propertyChangedHandler = PropertyChanged;
            if (propertyChangedHandler != null)
            {
                propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises the <see cref="GetViewModels"/> event which is handled on the DocumentViewBase
        /// to get one or more view models based on settings in <see cref="FindDocumentViewModel"/>.
        /// </summary>
        /// <param name="e">The <see cref="FindDocumentViewModel"/>.</param>
        protected void OnGetViewModels(FindDocumentViewModel e)
        {
            var getViewModels = GetViewModels;
            if (getViewModels != null)
            {
                getViewModels(this, e);
            }
        }

        /// <summary>
        /// Calls the <see cref="OnPublished"/> method implemented by the
        /// sub class which in turn calls the view model passing the args.
        /// </summary>
        protected void DataPublished()
        {
            ProcessAction(OnPublished);
        }

        /// <summary>
        /// Calls the <see cref="DataPublished"/>.
        /// </summary>
        /// <param name="param">This property is ignored.</param>
        private void OnRefresh(object param)
        {
            DataPublished();
        }

        /// <summary>
        /// Calls the <see cref="SaveDocument"/>.
        /// </summary>
        /// <param name="param">This argument is ignored.</param>
        private void OnSave(object param)
        {
            ProcessAction(SaveDocument);
        }

        /// <summary>
        /// Process an action implemented in the specialised classes the derive from <see cref="ViewModelBase"/>.
        /// </summary>
        /// <param name="action">The action to run.</param>
        protected void ProcessAction(Action action)
        {
            action.Invoke();
        }

        /// <summary>
        /// Clears the message collection.
        /// </summary>
        /// <param name="param">This argument is ignored.</param>
        private void OnClearMessages(object param)
        {
            ClearMessages();
        }

        private Category ConvertMessageTypeToLogCategory(MessageType type)
        {
            switch (type)
            {
                case MessageType.Error:
                    return Category.Exception;
                case MessageType.Warn:
                    return Category.Warn;
                case MessageType.Info:
                case MessageType.Question:
                    return Category.Info;
                default:
                    return Category.Debug;
            }
        }
    }
}