namespace Microsoft.Maui.Automation
{
    public interface IApplication
    {
        public Platform DefaultPlatform { get; }

        public Task<IEnumerable<Element>> GetElements(Platform platform, string? elementId = null, int childDepth = 0);

        public Task<string> GetProperty(Platform platform, string elementId, string propertyName);
    }
}