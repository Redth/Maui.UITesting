
namespace Microsoft.Maui.Automation.Remote
{
    public interface IRemoteAutomationService
    {
        public Task<RemoteElement[]> Children(Platform platform);
        public Task<RemoteElement?> Element(Platform platform, string elementId);
        public Task<RemoteElement[]> Descendants(Platform platform, string? elementId = null, IElementSelector? selector = null);
        public Task<IActionResult> Perform(Platform platform, string elementId, IAction action);
        public Task<object?> GetProperty(Platform platform, string elementId, string propertyName);
    }
}