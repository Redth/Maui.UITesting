namespace Microsoft.Maui.Automation
{
    public interface IApplicationInterop
    {
        public Task<IWindow[]> Windows();
        public Task<IWindow?> Window(string id);
        public Task<IWindow?> CurrentWindow();

        public Task<IView?> View(string windowId, string viewId);

        public IAsyncEnumerable<IView> Descendants(string windowId);

        public IAsyncEnumerable<IView> Descendants(string windowId, string elementId);

        public Task<IActionResult> Invoke(string windowId, string elementId, IAction action);

        public Task<object?> GetProperty(string windowId, string elementId, string propertyName);
    }
}