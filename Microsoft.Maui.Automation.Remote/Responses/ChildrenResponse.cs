using Microsoft.Maui.Automation.Remote;
using Newtonsoft.Json;

namespace Streamer
{
    public class ChildrenResponse : Response
    {
        public ChildrenResponse(RemoteElement[] result)
            => Result = result;

        [JsonProperty]
        public RemoteElement[] Result { get; set; }
    }
}