
namespace Microsoft.Maui.Automation.Remote
{
    public interface IRemoteAutomationService
    {
        public Task<RemoteWindow[]> Windows();
        public Task<RemoteWindow?> Window(string id);
        public Task<RemoteWindow?> CurrentWindow();

        public Task<RemoteView?> View(string windowId, string viewId);

        public Task<RemoteView[]?> WindowDescendants(string windowId);
        public Task<RemoteView[]?> ViewDescendants(string windowId, string viewId);

        public Task<IActionResult> Invoke(string windowId, string viewId, IAction action);

        public Task<object?> GetProperty(string windowId, string viewId, string propertyName);

        public Task Platform(Platform platform);
    }
}