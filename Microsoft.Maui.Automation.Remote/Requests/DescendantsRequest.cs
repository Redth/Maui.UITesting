using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Streamer
{
    public class DescendantsRequest : Request
    {
        public DescendantsRequest(Platform platform, string? elementId = null, IElementSelector? selector = null)
        {
            Method = nameof(IRemoteAutomationService.Descendants);
            Platform = platform;
            ElementId = elementId;
            Selector = selector;
        }

        [JsonProperty]
        public Platform Platform { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? ElementId { get; set; } = null;

        [JsonProperty]
        public IElementSelector? Selector { get;}
    }
}