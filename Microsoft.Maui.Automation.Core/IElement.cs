
using System.Text.Json.Serialization;

namespace Microsoft.Maui.Automation
{
	[JsonConverter(typeof(ElementConverter))]
    public interface IElement
    {
		public IApplication Application { get; }

		public Platform Platform { get; }

		public string? ParentId { get; }

		public bool Visible { get; }
		public bool Enabled { get; }
		public bool Focused { get; }


		[Newtonsoft.Json.JsonIgnore]
		[JsonIgnore]
		public object? PlatformElement { get; }

		[JsonIgnore]
		public IReadOnlyCollection<IElement> Children { get; }

		public string Id { get; }

		public string AutomationId { get; }

		public string Type { get; }
		public string FullType { get; }

		public string? Text { get; }

		public int Width { get; }
		public int Height { get; }
		public int X { get; }
		public int Y { get; }
	}
}
