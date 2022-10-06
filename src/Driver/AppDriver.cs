using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Maui.Automation.Driver;

public class AppDriver : Driver
{
	public AppDriver(IAutomationConfiguration configuration, ILoggerFactory? loggerProvider = null)
		: base(configuration, loggerProvider)
	{
		Driver = configuration.DevicePlatform switch
		{
			Platform.Android => new AndroidDriver(configuration, loggerProvider),
			Platform.Ios => new iOSDriver(configuration, loggerProvider),
			Platform.Tvos => new iOSDriver(configuration, loggerProvider),
			Platform.Maccatalyst => new MacDriver(configuration, loggerProvider),
			Platform.Winappsdk => new WindowsDriver(configuration, loggerProvider),
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

    public override Task<string?> GetProperty(Platform automationPlatform, string elementId, string propertyName)
        => Driver.GetProperty(automationPlatform, elementId, propertyName);

    public override Task<PerformActionResult> PerformAction(Platform automationPlatform, string action, string elementId, params string[] arguments)
		=> Driver.PerformAction(automationPlatform, action, elementId, arguments);

	public override Task InputText(IElement element, string text)
		=> Driver.InputText(element, text);

	public override Task ClearText(IElement element)
		=> Driver.ClearText(element);

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

	public override Task LongPress(IElement element)
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

	public override Task Tap(IElement element)
		=> Driver.Tap(element);

	public override ValueTask DisposeAsync()
		=> Driver.DisposeAsync();

	public override Task<string[]> Backdoor(Platform automationPlatform, string fullyQualifiedTypeName, string staticMethodName, string[] args)
		=> Driver.Backdoor(automationPlatform, fullyQualifiedTypeName, staticMethodName, args);

	public override Task<IEnumerable<IElement>> GetElements(Platform automationPlatform)
		=> Driver.GetElements(automationPlatform);

	public override Task Screenshot(string? filename = null)
		=> Driver.Screenshot(filename);
}
