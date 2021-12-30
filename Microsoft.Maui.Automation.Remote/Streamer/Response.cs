using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Streamer
{
    public class Response
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Error { get; set; }
    }
}