using Microsoft.Maui.Automation.Remote;

namespace Streamer
{
	public class ElementResponse : Response
    {
        public ElementResponse(RemoteElement? result)
        {
            Element = result;
        }

        public RemoteElement? Element { get; set; }
    }
}