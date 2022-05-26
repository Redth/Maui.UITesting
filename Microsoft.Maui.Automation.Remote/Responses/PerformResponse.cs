using Microsoft.Maui.Automation;

namespace Streamer
{
	public class PerformResponse : Response
    {
        public PerformResponse(IActionResult result)
            => Result = result;

        public IActionResult Result { get; set; }
    }
}