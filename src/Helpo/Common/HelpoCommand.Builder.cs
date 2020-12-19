using System;
using System.Threading.Tasks;
using MudBlazor;

namespace Helpo.Common
{
    public partial class HelpoCommand
    {
        public static HelpoCommandBuilder Async(Func<Task> execute)
        {
            return new HelpoCommandBuilder(execute ?? throw new ArgumentNullException(nameof(execute)));
        }
        
        public static HelpoCommandBuilder Sync(Action execute)
        {
            if (execute is null) throw new ArgumentNullException(nameof(execute));
            
            return Async(() =>
            {
                execute();
                return Task.CompletedTask;
            });
        }
        
        public class HelpoCommandBuilder
        {
            private readonly Func<Task> _execute;
            private Func<bool>? _canExecute;
            private Action? _stateHasChanged;
            private ISnackbar? _snackbar;

            public HelpoCommandBuilder(Func<Task> execute)
            {
                this._execute = execute ?? throw new ArgumentNullException(nameof(execute));
            }

            public HelpoCommandBuilder CanExecute(Func<bool> canExecute)
            {
                this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
                return this;
            }

            public HelpoCommandBuilder Component(Action stateHasChanged)
            {
                this._stateHasChanged = stateHasChanged;
                return this;
            }

            public HelpoCommandBuilder Snackbar(ISnackbar snackbar)
            {
                this._snackbar = snackbar;
                return this;
            }

            public Common.HelpoCommand Create()
            {
                return new Common.HelpoCommand(this._execute, this._canExecute, this._stateHasChanged, this._snackbar);
            }

            public static implicit operator Common.HelpoCommand(HelpoCommandBuilder builder)
            {
                if (builder is null) throw new ArgumentNullException(nameof(builder));
                
                return builder.Create();
            }
        }   
    }
}