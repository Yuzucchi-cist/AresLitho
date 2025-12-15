using System.Windows.Input;

namespace AresLitho.Commons
{
    /// <summary>
    /// A command that relays its functionality to other objects by invoking delegates.
    /// Reference: https://yossy51.com/mvvm-command-implementation/
    /// </summary>
    class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;        // Action of this command
        private readonly Func<T?, bool>? _canExecute;    // Condition to execute this command

        public RelayCommand(Action<T?> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = null;
        }

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute((T?)parameter);
        }

        public void Execute(object? parameter)
        {
            _execute((T?)parameter);
        }

        // Method to raise CanExecuteChanged event
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
