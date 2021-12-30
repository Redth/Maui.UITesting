using Newtonsoft.Json;

namespace Streamer
{
    public abstract class Request
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "method")]
        public string? Method { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "params")]
        public object?[] Args { get; set; }
    }
}