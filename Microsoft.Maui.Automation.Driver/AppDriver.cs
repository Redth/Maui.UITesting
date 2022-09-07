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
			Platform.Maccatalyst => new iOSDriver(configuration),
			Platform.Winappsdk => new WindowsDriver(configuration),
			_ => throw new NotSupportedException($"Unsupported Device Platform: '{configuration.DevicePlatform}'")
		};
	}

	public readonly IDriver Driver;

	public override string Name => Driver.Name;

	public override Task Back()
		=> Driver.Back();

	public override Task ClearAppState(string appId)
		=> Driver.ClearAppState(appId);

	public override Task<IEnumerable<Element>> FindElements(string propertyName, string pattern, bool isExpression = false, string ancestorId = "")
		=> Driver.FindElements(propertyName, pattern, isExpression, propertyName);

	public override Task<IDeviceInfo> GetDeviceInfo()
		=> Driver.GetDeviceInfo();

	public override Task<IEnumerable<Element>> GetElements()
		=> Driver.GetElements();

	public override Task<string> GetProperty(string elementId, string propertyName)
		=> Driver.GetProperty(elementId, propertyName);

	public override Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
		=> Driver.PerformAction(action, elementId, arguments);

	public override Task InputText(string text)
		=> Driver.InputText(text);

	public override Task InstallApp(string file, string appId)
		=> Driver.InstallApp(file, appId);

	public override Task KeyPress(char keyCode)
		=> Driver.KeyPress(keyCode);

	public override Task LaunchApp(string appId)
		=> Driver.LaunchApp(appId);

	public override Task LongPress(int x, int y)
		=> Driver.LongPress(x, y);

	public override Task OpenUri(string uri)
		=> Driver.OpenUri(uri);

	public override Task PullFile(string appId, string remoteFile, string localDirectory)
		=> Driver.PullFile(appId, remoteFile, localDirectory);

	public override Task PushFile(string appId, string localFile, string destinationDirectory)
		=> Driver.PushFile(appId, localFile, destinationDirectory);

	public override Task RemoveApp(string appId)
		=> Driver.RemoveApp(appId);

	public override Task StopApp(string appId)
		=> Driver.StopApp(appId);

	public override Task Swipe((int x, int y) start, (int x, int y) end)
		=> Driver.Swipe(start, end);

	public override Task Tap(int x, int y)
		=> Driver.Tap(x, y);

	public override Task Tap(Element element)
		=> Driver.PerformAction(Actions.Tap, element.Id);

	public override void Dispose()
	{
		Driver.Dispose();
	}
}
