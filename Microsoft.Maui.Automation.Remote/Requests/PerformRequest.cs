using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;
using Newtonsoft.Json;

namespace Streamer
{
    public class PerformRequest : Request
    {
        public PerformRequest(Platform platform, string elementId, IAction action)
        {
            Method = nameof(IRemoteAutomationService.Perform);
            Platform = platform;
            ElementId = elementId;
            Action = action;
        }

        [JsonProperty]
        public Platform Platform { get; set; }

        [JsonProperty]
        public string ElementId { get; set; }

        [JsonProperty]
        public IAction Action { get; set; }
    }
}