using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;
using Newtonsoft.Json;

namespace Streamer
{
    public class GetPropertyRequest : Request
    {
        public GetPropertyRequest(Platform platform, string elementId, string propertyName)
        {
            Method = nameof(IRemoteAutomationService.GetProperty);
            Platform = platform;
            ElementId = elementId;
            PropertyName = propertyName;
        }

        [JsonProperty]
        public Platform Platform { get; set; }

        [JsonProperty]
        public string ElementId { get; set; }

        [JsonProperty]
        public string PropertyName { get; set; }
    }
}