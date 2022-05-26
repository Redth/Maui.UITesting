using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;

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

        public Platform Platform { get; set; }

        public string ElementId { get; set; }
    }
}