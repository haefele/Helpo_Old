using System;
using Helpo.Shared;
using Microsoft.AspNetCore.Components;

namespace Helpo
{
    public abstract class HelpoPage : ComponentBase, IDisposable
    {
        private IDisposable? _titleDisposable;
        
        [CascadingParameter]
        public TitleComponent? TitleComponent { get; set; }
        
        public virtual string? Title { get; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (string.IsNullOrWhiteSpace(this.Title) == false)
                _titleDisposable = this.TitleComponent?.AddTitle(this.Title);
        }

        public virtual void Dispose()
        {
            this._titleDisposable?.Dispose();
        }
    }
}