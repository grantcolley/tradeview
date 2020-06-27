//-----------------------------------------------------------------------
// <copyright file="ViewModelCommand.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Host.ViewModel
{
    /// <summary>
    /// Standard command object for use by view models.
    /// </summary>
    public class ViewModelCommand : ICommand
    {
        private readonly Predicate<object> canExecute;
        private readonly Action<object> execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute delegate.</param>
        public ViewModelCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelCommand"/> class.
        /// </summary>
        /// <param name="execute">The delegate to execute.</param>
        /// <param name="canExecute">The delegate that determines whether or not to execute.</param>
        public ViewModelCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Notification that the status of delegate that 
        /// determines whether or not to execute has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Delegate determines whether or not execute can take place.
        /// </summary>
        /// <param name="parameter">The value to evaluate.</param>
        /// <returns>True if successful, else returns false.</returns>
        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
            {
                return true;
            }

            return canExecute(parameter);
        }

        /// <summary>
        /// Calls the execute delegate.
        /// </summary>
        /// <param name="parameter">The parameter to pass the execute delegate.</param>
        public void Execute(object parameter)
        {
            execute(parameter);
        }

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var canExecuteChanged = CanExecuteChanged;
            if (canExecuteChanged != null)
            {
                canExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
