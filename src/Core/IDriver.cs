namespace Microsoft.Maui.Automation.Driver
{
	public interface IDriver : IDisposable
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

		Task LongPress(Element element);

		Task Tap(Element element);

		Task KeyPress(char keyCode);

		Task Swipe((int x, int y) start, (int x, int y) end);

		Task Back();

		Task InputText(Element element, string text);

		Task ClearText(Element element);

		Task OpenUri(string uri);

		Task<IEnumerable<Element>> GetElements();

		Task<string?> GetProperty(string elementId, string propertyName);

		Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments);

		Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args);

		Task Screenshot(string path);
	}
}