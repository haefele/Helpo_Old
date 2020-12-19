using System;

namespace Helpo.Common
{
    public class DisposableAction : IDisposable
    {
        private readonly Action _action;

        public DisposableAction(Action action)
        {
            Guard.NotNull(action, nameof(action));
            
            this._action = action;
        }
        
        public void Dispose()
        {
            this._action();
        }
    }
}