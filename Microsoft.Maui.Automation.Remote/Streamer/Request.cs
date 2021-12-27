using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Streamer
{
    public class Request
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "method")]
        public string? Method { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "params")]
        public JToken[]? Args { get; set; }
    }
}