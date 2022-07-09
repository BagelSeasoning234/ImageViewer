using System;
using System.Windows.Input;

namespace ImageViewer.Core
{
    class RelayCommand : ICommand
    {

        #region Private Members

        // The action to execute.
        private Action<object> m_Execute;
        // Returns whether the event can be executed.
        private Func<object, bool> m_CanExecute;

        #endregion

        #region Public Methods

        /// <summary>
        /// Informs the command manager when CanExecute changes.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Creates a new relay command.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">Whether the action can currently execute. Null by default.</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            m_Execute = execute;
            m_CanExecute = canExecute;
        }

        /// <summary>
        /// Returns whether the action can currently execute.
        /// </summary>
        /// <param name="parameter">The action to check.</param>
        /// <returns>True if the action can execute. False otherwise.</returns>
        public bool CanExecute(object parameter)
        {
            return m_CanExecute == null || m_CanExecute(parameter);
        }

        /// <summary>
        /// Executes the specified action.
        /// </summary>
        /// <param name="parameter">The action to execute.</param>
        public void Execute(object parameter)
        {
            m_Execute(parameter);
        }

        #endregion
    }
}
