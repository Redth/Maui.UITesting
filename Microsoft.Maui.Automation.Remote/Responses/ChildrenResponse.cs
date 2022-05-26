using Microsoft.Maui.Automation.Remote;

namespace Streamer
{
    public class ChildrenResponse : Response
    {
        public ChildrenResponse(RemoteElement[] result)
            => Result = result;

        public RemoteElement[] Result { get; set; }
    }
}