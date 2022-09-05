namespace Microsoft.Maui.Automation
{
    public static class Actions
    {
        public const string Tap = "TAP";

    }
    public class PerformActionResult
    {
        public static PerformActionResult Ok(string result = "")
            => new PerformActionResult { Result = result, Status = 0 };

        public static PerformActionResult Error(string result = "", int status = -1)
            => new PerformActionResult { Result = result, Status = status };

        public int Status { get; set; }
        public string Result { get; set; } = string.Empty;
    }
}