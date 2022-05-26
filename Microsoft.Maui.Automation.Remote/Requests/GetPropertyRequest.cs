using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;

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

        public Platform Platform { get; set; }

        public string ElementId { get; set; }

        public string PropertyName { get; set; }
    }
}