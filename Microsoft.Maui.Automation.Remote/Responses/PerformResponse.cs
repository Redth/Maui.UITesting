using Microsoft.Maui.Automation;
using Newtonsoft.Json;

namespace Streamer
{
    public class PerformResponse : Response
    {
        public PerformResponse(IActionResult result)
            => Result = result;

        [JsonProperty]
        public IActionResult Result { get; set; }
    }
}