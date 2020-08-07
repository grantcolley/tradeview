//-----------------------------------------------------------------------
// <copyright file="Shell.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using CommonServiceLocator;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.View;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.ViewModel;
using Prism.Logging;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;

namespace DevelopmentInProgress.TradeView.Wpf.Host
{
    /// <summary>
    /// The main window.
    /// </summary>
    public partial class Shell : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Determines the operation to perform on all visible documents.
        /// </summary>
        private enum ActionAllEnum
        {
            RefreshAll,
            SaveAll
        }

        /// <summary>
        /// Determines the operation to perform on the active document.
        /// </summary>
        private enum ActionActiveEnum
        {
            Save
        }

        /// <summary>
        /// Determines whether the tool bar is vibile or not.
        /// </summary>
        private static DependencyProperty IsToolBarVisibleProperty = DependencyProperty.Register("IsToolBarVisible",
            typeof (bool), typeof (Shell));

        private ILoggerFacade logger;

        private ModulesNavigationViewModel modulesNavigationViewModel;

        /// <summary>
        /// Initializes a new instance of the Shell class.
        /// </summary>
        public Shell()
        {
            InitializeComponent();

            var appSettings = ConfigurationManager.AppSettings;
            var isShellToolBarVisible = appSettings["IsShellToolBarVisible"];
            ShellToolBar.Visibility = isShellToolBarVisible.ToUpper().Equals("TRUE")
                ? Visibility.Visible
                : Visibility.Collapsed;

            Title = $"Trade View : {Environment.UserName}";

            logger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ModulesNavigationViewModel ModulesNavigationViewModel
        {
            get { return modulesNavigationViewModel; }
            set
            {
                if(modulesNavigationViewModel != value)
                {
                    modulesNavigationViewModel = value;
                    OnPropertyChanged(nameof(ModulesNavigationViewModel));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the tool bar is vibile or not.
        /// </summary>
        public bool IsToolBarVisible
        {
            get { return (bool)GetValue(IsToolBarVisibleProperty); }
            set { SetValue(IsToolBarVisibleProperty, value); }
        }

        /// <summary>
        /// Close the application.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Save the current active document.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            ActionActive(ActionActiveEnum.Save);
        }

        /// <summary>
        /// Save all visible documents.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SaveAllClick(object sender, RoutedEventArgs e)
        {
            ActionAll(ActionAllEnum.SaveAll);
        }

        /// <summary>
        /// Refresh all visible documents.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event arguments.</param>
        private void RefreshAllClick(object sender, RoutedEventArgs e)
        {
            ActionAll(ActionAllEnum.RefreshAll);
        }

        /// <summary>
        /// Open the log file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OpenLogClick(object sender, RoutedEventArgs e)
        {
            string filePath = ConfigurationManager.AppSettings["serilog:write-to:File.path"].ToString();
            var dirPath = filePath.Substring(0, filePath.LastIndexOf('\\'));
            var directory = new DirectoryInfo(dirPath);
            var logFile = directory.GetFiles()
                .Where(f => f.Name.Contains("DevelopmentInProgress.TradeView.Wpf.Trading"))
                .OrderByDescending(f => f.LastWriteTime).First();

            string logFileReader = ConfigurationManager.AppSettings["LogFileReader"].ToString();
            Process.Start(logFileReader, logFile.FullName);
        }
        
        /// <summary>
        /// Performs the specified action on the currently active document.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        private void ActionActive(ActionActiveEnum action)
        {
            var documentPane = dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (documentPane == null)
            {
                return;
            }

            var layoutContent = documentPane.Children.FirstOrDefault(l => l.IsActive || l.IsSelected);
            if (layoutContent != null
                && layoutContent.Content is DocumentViewHost)
            {
                var viewModel = ((DocumentViewHost)layoutContent.Content).DataContext as DocumentViewModel;
                if (viewModel != null)
                {
                    if (action == ActionActiveEnum.Save)
                    {
                        viewModel.Save.Execute(null);
                    }
                }
            }
        }

        /// <summary>
        /// Performs the specified action on all visible documents.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        private void ActionAll(ActionAllEnum action)
        {
            var documentPane = dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (documentPane == null)
            {
                return;
            }

            // Note only the children for the current active module are
            // returned i.e. only the visible documents are returned.
            foreach (LayoutContent layoutContent in documentPane.Children)
            {
                if (layoutContent.Content is DocumentViewHost)
                {
                    var viewModel = ((DocumentViewHost)layoutContent.Content).DataContext as ViewModelBase;
                    if (viewModel != null)
                    {
                        if (action == ActionAllEnum.RefreshAll)
                        {
                            viewModel.Refresh.Execute(null);
                        }
                        else if (action == ActionAllEnum.SaveAll)
                        {
                            viewModel.Save.Execute(null);
                        }
                    }
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
