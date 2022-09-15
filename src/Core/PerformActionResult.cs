namespace Microsoft.Maui.Automation
{
	public static class Actions
	{
		public const string Tap = "TAP";

	}
	public partial class PerformActionResult
	{
		public const int SuccessStatus = 0;
		public const int ErrorStatus = -1;

		public static PerformActionResult Ok(string result = "")
			=> new PerformActionResult { Result = result, Status = SuccessStatus };

		public static PerformActionResult Error(string result = "", int status = ErrorStatus)
			=> new PerformActionResult { Result = result, Status = status };

		public int Status { get; set; } = ErrorStatus;

		public string Result { get; set; } = string.Empty;
	}
}