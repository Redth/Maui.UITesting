using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;
using Newtonsoft.Json;

namespace Streamer
{
    public class ElementRequest : Request
    {
        public ElementRequest(Platform platform, string elementId)
        {
            Method = nameof(IRemoteAutomationService.Element);
            Platform = platform;
            ElementId = elementId;
        }

        [JsonProperty]
        public Platform Platform { get; set; }

        [JsonProperty]
        public string ElementId { get; set; }
    }
}