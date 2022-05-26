namespace Microsoft.Maui.Automation
{
    public class NullApplication : IApplication
    {
        public NullApplication(Platform platform = Platform.MAUI)
            => DefaultPlatform = platform;

        public Platform DefaultPlatform { get; }

        public Task<IEnumerable<IElement>> Children(Platform platform)
            => Task.FromResult(Enumerable.Empty<IElement>());

        public Task<IEnumerable<IElement>> Descendants(Platform platform, string? ofElementId = null, IElementSelector? selector = null)
            => Task.FromResult(Enumerable.Empty<IElement>());

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