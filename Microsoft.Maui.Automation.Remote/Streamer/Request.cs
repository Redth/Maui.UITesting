using Microsoft.Maui.Automation;

namespace Streamer
{
	[System.Text.Json.Serialization.JsonConverter(typeof(PolyJsonConverter))]
	[JsonKnownType(typeof(ChildrenRequest), nameof(ChildrenRequest))]
	[JsonKnownType(typeof(DescendantsRequest), nameof(DescendantsRequest))]
	[JsonKnownType(typeof(ElementRequest), nameof(ElementRequest))]
	[JsonKnownType(typeof(GetPropertyRequest), nameof(GetPropertyRequest))]
	[JsonKnownType(typeof(PerformRequest), nameof(PerformRequest))]
	public abstract class Request
    {
        public int Id { get; set; }

        public string? Method { get; set; }

        public object?[] Args { get; set; }
    }
}