using Microsoft.Maui.Automation.Remote;

namespace Streamer
{
    public class DescendantsResponse : Response
    {
        public DescendantsResponse(RemoteElement[] result)
        {
            Result = result;
        }

        public RemoteElement[] Result { get; set; }
    }
}