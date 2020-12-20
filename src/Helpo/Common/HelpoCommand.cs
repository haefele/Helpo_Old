using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MudBlazor;

namespace Helpo.Common
{
    public class HelpoCommand : ICommand
    {
        #region Fields
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;
        private readonly Func<Task>? _stateHasChanged;
        private readonly Func<ISnackbar>? _snackbarAccessor;
        #endregion

        #region Properties
        public bool IsExecuting { get; private set; }
        #endregion

        #region Constructors
        public HelpoCommand(Func<Task> execute, Func<bool>? canExecute, Func<Task>? stateHasChanged, Func<ISnackbar>? snackbarAccessor)
        {
            Guard.NotNull(execute, nameof(execute));
            // canExecute
            // stateHasChanged
            // snackbarAccessor
            
            this._execute = this.Wrap(execute);
            this._canExecute = canExecute;
            this._stateHasChanged = stateHasChanged;
            this._snackbarAccessor = snackbarAccessor;
        }
        #endregion

        #region Methods
        public bool CanExecute => ((ICommand)this).CanExecute(null);
        
        public void Execute()
        {
            _ = this.ExecuteAsync();
        }

        public async Task ExecuteAsync()
        {
            if (((ICommand)this).CanExecute(null) == false)
                return;

            try
            {
                this.IsExecuting = true;
                await (this._stateHasChanged?.Invoke() ?? Task.CompletedTask);
                await this._execute();
            }
            finally
            {
                this.IsExecuting = false;
                await (this._stateHasChanged?.Invoke() ?? Task.CompletedTask);
            }
        }
        #endregion

        #region Private Methods
        private Func<Task> Wrap(Func<Task> execute)
        {
            return async () =>
            {
                try
                {
                    await execute();
                }
                catch (Exception exception)
                {
                    if (this._snackbarAccessor is null)
                        throw;
                    
                    this._snackbarAccessor?.Invoke().Add(exception.Message, Severity.Error);
                }
            };
        }
        #endregion

        #region Implementation of ICommand
        bool ICommand.CanExecute(object? parameter)
        {
            if (this.IsExecuting)
                return false;

            return this._canExecute?.Invoke() ?? true;
        }

        void ICommand.Execute(object? parameter)
        {
            this.Execute();
        }

#pragma warning disable 67
        private event EventHandler? CanExecuteChanged;
#pragma warning restore 67
        event EventHandler? ICommand.CanExecuteChanged
        {
            add => this.CanExecuteChanged += value;
            remove => this.CanExecuteChanged -= value;
        }
        #endregion
    }
}