
using Newtonsoft.Json;

namespace Microsoft.Maui.Automation
{
    public interface IElement
    {
		public Platform Platform { get; }

		[JsonIgnore]
		public object? PlatformElement { get; }

		public IView[] Children { get; }

		public string Id { get; }

		public string AutomationId { get; }

		public string Type { get; }

		public string? Text { get; }

		public int Width { get; }
		public int Height { get; }
	}
}
