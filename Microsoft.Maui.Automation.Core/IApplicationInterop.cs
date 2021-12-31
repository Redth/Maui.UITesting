namespace Microsoft.Maui.Automation
{
    public interface IApplication
    {
        public Platform DefaultPlatform { get; }

        public Task<IEnumerable<IElement>> Children(Platform platform);

        public Task<IElement?> Element(Platform platform, string elementId);

        public Task<IEnumerable<IElement>> Descendants(Platform platform, string? ofElementId = null, IElementSelector? selector = null);

        public Task<IActionResult> Perform(Platform platform, string elementId, IAction action);

        public Task<object?> GetProperty(Platform platform, string elementId, string propertyName);
    }
}