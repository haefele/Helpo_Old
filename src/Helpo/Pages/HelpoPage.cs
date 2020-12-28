using System;
using System.Threading.Tasks;
using Helpo.Common;
using Helpo.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Helpo.Pages
{
    public abstract class HelpoPage : ComponentBase, IDisposable
    {
        private IDisposable? _titleDisposable;
        
        [CascadingParameter]
        public TitleComponent? TitleComponent { get; set; }
        
        public virtual string? Title { get; }

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (string.IsNullOrWhiteSpace(this.Title) == false)
                _titleDisposable = this.TitleComponent?.AddTitle(this.Title);
        }

        protected HelpoCommand CreateCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            Guard.NotNull(execute, nameof(execute));
            // canExecute
            
            return new(execute, canExecute, () => this.InvokeAsync(this.StateHasChanged), () => this.Snackbar);
        }

        public virtual void Dispose()
        {
            this._titleDisposable?.Dispose();
        }
    }
}