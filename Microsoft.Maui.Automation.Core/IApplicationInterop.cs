namespace Microsoft.Maui.Automation
{
    public interface IApplicationInterop
    {
        public Task<IWindow[]> Windows();
        public Task<IWindow?> Window(string id);
        public Task<IWindow?> CurrentWindow();

        public Task<IView?> View(string windowId, string viewId);

        public IAsyncEnumerable<IView> Descendants(string windowId, string? viewId = null, IViewSelector? selector = null);

        public Task<IActionResult> Invoke(string windowId, string viewId, IAction action);

        public Task<object?> GetProperty(string windowId, string viewId, string propertyName);
    }
}