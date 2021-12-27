using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Streamer
{
    public class Response
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "result")]
        public JToken? Result { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "error")]
        public string? Error { get; set; }
    }
}