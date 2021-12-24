
namespace Microsoft.Maui.Automation.Remote
{
    public interface IRemoteAutomationService
    {
        public Task<RemoteWindow[]> Windows();
        public Task<RemoteWindow?> Window(string id);
        public Task<RemoteWindow?> CurrentWindow();

        public Task<RemoteView?> View(string windowId, string viewId);

        public IAsyncEnumerable<RemoteView> Descendants(string windowId);

        public IAsyncEnumerable<RemoteView> Descendants(string windowId, string elementId);

        public Task<RemoteView[]> Tree(string windowId);
        public Task<RemoteView[]> Tree(string windowId, string elementId);

        public Task<IActionResult> Invoke(string windowId, string elementId, IAction action);

        public Task<object?> GetProperty(string windowId, string elementId, string propertyName);

        public Task Platform(Platform platform);
    }
}