using Microsoft.Maui.Automation.Remote;
using Newtonsoft.Json;

namespace Streamer
{
    public class ElementResponse : Response
    {
        public ElementResponse(RemoteElement? result)
        {
            Element = result;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public RemoteElement? Element { get; set; }
    }
}