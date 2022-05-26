
using Microsoft.Maui.Automation;

namespace Streamer
{
	[System.Text.Json.Serialization.JsonConverter(typeof(PolyJsonConverter))]
	[JsonKnownType(typeof(ChildrenResponse), nameof(ChildrenResponse))]
	[JsonKnownType(typeof(DescendantsResponse), nameof(DescendantsResponse))]
	[JsonKnownType(typeof(ElementResponse), nameof(ElementResponse))]
	[JsonKnownType(typeof(GetPropertyResponse), nameof(GetPropertyResponse))]
	[JsonKnownType(typeof(PerformResponse), nameof(PerformResponse))]
	public class Response
    {
        public int Id { get; set; }

        public string? Error { get; set; }
    }
}