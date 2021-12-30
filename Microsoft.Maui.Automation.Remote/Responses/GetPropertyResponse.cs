using Newtonsoft.Json;

namespace Streamer
{
    public class GetPropertyResponse : Response
    {
        public GetPropertyResponse(object? result)
            => Result = result;

        [JsonProperty]
        public object? Result { get; set; }
    }
}