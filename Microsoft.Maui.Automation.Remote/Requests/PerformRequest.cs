using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;

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

        public Platform Platform { get; set; }

        public string ElementId { get; set; }

        public IAction Action { get; set; }
    }
}