using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Streamer
{
    public class ChildrenRequest : Request
    {
        public ChildrenRequest(Platform platform)
        {
            Method = nameof(IRemoteAutomationService.Children);
            Platform = platform;
        }

        [JsonProperty]
        public Platform Platform { get; set; }
    }
}