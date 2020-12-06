using System;

namespace Helpo.Shared.Toolbars
{
    public class ToolbarButton
    {
        private readonly Action? _onClick;

        public static ToolbarButton Create(string icon, string text, Action? onClick = null)
            => new ToolbarButton(icon, text, onClick);
        
        private ToolbarButton(string icon, string text, Action? onClick)
        {
            this.Icon = icon ?? throw new ArgumentNullException(nameof(icon));
            this.Text = text ?? throw new ArgumentNullException(nameof(icon));
            this._onClick = onClick;
        }
        
        public string Icon { get; }
        public string Text { get; }

        public void Click()
        {
            this._onClick?.Invoke();
        }
    }
}