using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;

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

        public Platform Platform { get; set; }

        public string? ElementId { get; set; } = null;

        public IElementSelector? Selector { get;}
    }
}