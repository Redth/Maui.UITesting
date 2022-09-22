using System;

namespace Microsoft.Maui.Automation.Driver;

public class AppDriver : Driver
{
	public AppDriver(IAutomationConfiguration configuration)
		: base(configuration)
	{
		Driver = configuration.DevicePlatform switch
		{
			Platform.Android => new AndroidDriver(configuration),
			Platform.Ios => new iOSDriver(configuration),
			Platform.Tvos => new iOSDriver(configuration),
			Platform.Maccatalyst => new MacDriver(configuration),
			Platform.Winappsdk => new WindowsDriver(configuration),
			_ => throw new NotSupportedException($"Unsupported Device Platform: '{configuration.DevicePlatform}'")
		};
	}

	public readonly IDriver Driver;

	public override string Name
		=> Driver.Name;

	public override Task Back()
		=> Driver.Back();

	public override Task ClearAppState()
		=> Driver.ClearAppState();

	public override Task<IDeviceInfo> GetDeviceInfo()
		=> Driver.GetDeviceInfo();

	public override Task<string?> GetProperty(string elementId, string propertyName)
		=> Driver.GetProperty(elementId, propertyName);

	public override Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
		=> Driver.PerformAction(action, elementId, arguments);

	public override Task InputText(Element element, string text)
		=> Driver.InputText(element, text);

	public override Task InstallApp()
		=> Driver.InstallApp();

	public override Task KeyPress(char keyCode)
		=> Driver.KeyPress(keyCode);

	public override Task Start(bool forceReInstall = false, bool clearAppState = false)
		=> Driver.Start(forceReInstall, clearAppState);

	public override Task LaunchApp()
		=> Driver.LaunchApp();

	public override Task LongPress(int x, int y)
		=> Driver.LongPress(x, y);

	public override Task LongPress(Element element)
		=> Driver.LongPress(element);

	public override Task OpenUri(string uri)
		=> Driver.OpenUri(uri);

	public override Task PullFile(string remoteFile, string localDirectory)
		=> Driver.PullFile(remoteFile, localDirectory);

	public override Task PushFile(string localFile, string destinationDirectory)
		=> Driver.PushFile(localFile, destinationDirectory);

	public override Task RemoveApp()
		=> Driver.RemoveApp();

	public override Task StopApp()
		=> Driver.StopApp();

	public override Task Swipe((int x, int y) start, (int x, int y) end)
		=> Driver.Swipe(start, end);

	public override Task Tap(int x, int y)
		=> Driver.Tap(x, y);

	public override Task Tap(Element element)
		=> Driver.Tap(element);

	public override void Dispose()
		=> Driver.Dispose();

	public override Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args)
		=> Driver.Backdoor(fullyQualifiedTypeName, staticMethodName, args);

	public override Task<IEnumerable<Element>> GetElements()
		=> Driver.GetElements();
}
