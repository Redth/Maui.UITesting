namespace Microsoft.Maui.Automation.Driver
{
	public interface IDriver : IAsyncDisposable
	{
		string Name { get; }

		IAutomationConfiguration Configuration { get; }

		Task<IDeviceInfo> GetDeviceInfo();

		Task Start(bool forceReInstall = false, bool clearAppState = false);

		Task InstallApp();

		Task RemoveApp();

		Task LaunchApp();
		Task StopApp();

		Task ClearAppState();

		Task PushFile(string localFile, string destinationDirectory);
		Task PullFile(string remoteFile, string localDirectory);

		Task Tap(int x, int y);

		Task LongPress(int x, int y);

		Task LongPress(IElement element);

		Task Tap(IElement element);

		Task KeyPress(char keyCode);

		Task Swipe((int x, int y) start, (int x, int y) end);

		Task Back();

		Task InputText(IElement element, string text);

		Task ClearText(IElement element);

		Task OpenUri(string uri);

		Task<IEnumerable<IElement>> GetElements();
		Task<IEnumerable<IElement>> GetElements(Platform automationPlatform);

		Task<string?> GetProperty(string elementId, string propertyName);
		Task<string?> GetProperty(Platform automationPlatform, string elementId, string propertyName);

		Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments);
		Task<PerformActionResult> PerformAction(Platform automationPlatform, string action, string elementId, params string[] arguments);

		Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args);
		Task<string[]> Backdoor(Platform automationPlatform, string fullyQualifiedTypeName, string staticMethodName, string[] args);

		Task Screenshot(string? filename = null);
	}
}