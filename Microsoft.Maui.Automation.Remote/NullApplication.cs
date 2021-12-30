namespace Microsoft.Maui.Automation.Remote
{
    public class NullApplication : IApplication
    {
        public NullApplication(Platform platform = Platform.MAUI)
            => DefaultPlatform = platform;

        public Platform DefaultPlatform { get; }

        public async IAsyncEnumerable<IElement> Children(Platform platform)
        {
            yield break;
        }

        public async IAsyncEnumerable<IElement> Descendants(Platform platform, string? ofElementId = null, IElementSelector? selector = null)
        {
            yield break;
        }

        public Task<IElement?> Element(Platform platform, string elementId)
        {
            return Task.FromResult<IElement?>(null);
        }

        public Task<object?> GetProperty(Platform platform, string elementId, string propertyName)
        {
            return Task.FromResult<object?>(null);
        }

        public Task<IActionResult> Perform(Platform platform, string elementId, IAction action)
        {
            return Task.FromResult<IActionResult>(new ActionResult(ActionResultStatus.Unknown));
        }
    }
}