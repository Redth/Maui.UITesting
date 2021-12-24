namespace Microsoft.Maui.Automation
{
    public class ActionResult : IActionResult
    {
        public ActionResult(ActionResultStatus status, string? error = null)
        {
            Status = status;
            Error = error;
        }

        public ActionResultStatus Status { get; }
        public string? Error { get; }
    }

    public interface IActionResult
    {
        public ActionResultStatus Status { get; }
        public string? Error { get; }
    }
}
