namespace Microsoft.Maui.Automation
{
	public static class Actions
	{
		public const string Tap = "TAP";
		public const string GetProperty = "GETPROPERTY";
		public const string Backdoor = "BACKDOOR";
	}

	public partial class PerformActionResult
	{
		public const int SuccessStatus = 0;
		public const int ErrorStatus = -1;

		public static PerformActionResult Ok(params string[] results)
			=> new PerformActionResult { Results = results ?? new string[0], Status = SuccessStatus };

		public static PerformActionResult Error(int status, params string[] results)
			=> new PerformActionResult { Results = results ?? new string[0], Status = status };

        public static PerformActionResult Error(params string[] results)
            => new PerformActionResult { Results = results ?? new string[0], Status = ErrorStatus };

        public int Status { get; set; } = ErrorStatus;

		public string[] Results { get; set; } = new string[0];
	}
}