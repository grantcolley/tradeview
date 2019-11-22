//-----------------------------------------------------------------------
// <copyright file="WpfCommand.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2013. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.Command
{
    /// <summary>
    /// A custom command.
    /// </summary>
    public class WpfCommand : ICommand
    {
        private readonly Predicate<object> canExecute;
        private readonly Action<object> execute;

        /// <summary>
        /// Initializes a new instance of the WpfCommand class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        public WpfCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WpfCommand class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        /// <param name="canExecute">A value indicating whether the action can be executed.</param>
        public WpfCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// The CanExecuteChanged event.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Evaluates the parameter to determine whether the cammand can execute.
        /// </summary>
        /// <param name="parameter">The parameter to evaluate.</param>
        /// <returns>True if the command can execute, else returns false.</returns>
        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
            {
                return true;
            }

            return canExecute(parameter);
        }

        /// <summary>
        /// Executes the action passing the specified parameter.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            execute(parameter);
        }

        /// <summary>
        /// Raises the can execute changed event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
