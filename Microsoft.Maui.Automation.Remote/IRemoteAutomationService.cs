
namespace Microsoft.Maui.Automation.Remote
{
    public interface IRemoteAutomationService
    {
        public Task<RemoteWindow[]> Windows();
        public Task<RemoteWindow?> Window(string id);
        public Task<RemoteWindow?> CurrentWindow();

        public Task<RemoteView?> View(string windowId, string viewId);

        public Task<RemoteView[]> Descendants(string windowId, string? viewId = null);

        public Task<IActionResult> Invoke(string windowId, string viewId, IAction action);

        public Task<object?> GetProperty(string windowId, string viewId, string propertyName);

        public Task Platform(Platform platform);
    }
}