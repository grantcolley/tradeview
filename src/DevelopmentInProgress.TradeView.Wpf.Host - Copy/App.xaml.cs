//-----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using DevelopmentInProgress.TradeView.Wpf.Host.Navigation;
using Microsoft.Practices.ServiceLocation;
using Prism.Logging;

namespace DevelopmentInProgress.TradeView.Wpf.Host
{
    /// <summary>
    /// The application class.
    /// </summary>
    public partial class App : Application
    {
        private ILoggerFacade logger;
        
        /// <summary>
        /// Initializes a new instance of the App class.
        /// </summary>
        public App()
        {
            // Fix for memory leak in WPF present in versions of the framework up to and 
            // including .NET 3.5 SP1. This occurs because of the way WPF selects which 
            // HWND to use to send messages from the render thread to the UI thread.
            // http://stackoverflow.com/questions/1705849/wpf-memory-leak-on-xp-cmilchannel-hwnd
            new HwndSource(new HwndSourceParameters());
        }

        /// <summary>
        /// Start up method which loads and runs the bootstrapper.
        /// </summary>
        /// <param name="e">Startup event arguments.</param>
        protected override  void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            Current.DispatcherUnhandledException += DispatcherUnhandledException;

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();

            logger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
        }

        /// <summary>
        /// DispatcherUnhandledException is raised by an Application for each 
        /// exception that is unhandled by code running on the main UI thread.
        /// The error is logged, displayed in a window and then marked as handled.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Exception event arguments.</param>
        private new void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogException(e.Exception);

            var modalManager = ServiceLocator.Current.GetInstance<ModalNavigator>();
            modalManager.ShowError(e.Exception);
            
            e.Handled = true;
        }

        /// <summary>
        /// This event allows the application to log information about an 
        /// unhandled exception before the system default handler reports 
        /// the exception to the user and terminates the application.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Exception event arguments.</param>
        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                LogException(ex);
            }
            else if (e.ExceptionObject != null)
            {
                LogException(new Exception(e.ExceptionObject.ToString()));
            }
        }

        /// <summary>
        /// Logs an exception to the application log file.
        /// </summary>
        /// <param name="e">The exception to log.</param>
        private void LogException(Exception e)
        {
            logger.Log(e.Message, Category.Exception, Priority.High);
            logger.Log(e.StackTrace, Category.Exception, Priority.High);
            if (e.InnerException != null)
            {
                LogException(e.InnerException);
            }
        }
    }
}
