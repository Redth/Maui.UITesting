using Microsoft.Maui.Automation;
using Microsoft.Maui.Automation.Remote;

namespace Streamer
{
	public class ChildrenRequest : Request
    {
        public ChildrenRequest(Platform platform)
        {
            Method = nameof(IRemoteAutomationService.Children);
            Platform = platform;
        }

        public Platform Platform { get; set; }
    }
}