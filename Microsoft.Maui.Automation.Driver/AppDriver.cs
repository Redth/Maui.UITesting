using System;

namespace Microsoft.Maui.Automation.Driver;

public class AppDriver : IDriver
{
	public AppDriver(IAutomationConfiguration configuration)
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

    public IAutomationConfiguration Configuration => Driver!.Configuration;

    public string Name => Driver.Name;

    public Task Back()
        => Driver.Back();

    public Task ClearAppState(string appId)
        => Driver.ClearAppState(appId);

    public Task<IEnumerable<Element>> FindElements(Platform platform, string propertyName, string pattern, bool isExpression = false, string ancestorId = "")
        => Driver.FindElements(platform, propertyName, pattern, isExpression, propertyName);

    public Task<IDeviceInfo> GetDeviceInfo()
        => Driver.GetDeviceInfo();

    public Task<IEnumerable<Element>> GetElements(Platform platform)
        => Driver.GetElements(platform);

    public Task<string> GetProperty(Platform platform, string elementId, string propertyName)
        => Driver.GetProperty(platform, elementId, propertyName);

    public Task InputText(string text)
        => Driver.InputText(text);

    public Task InstallApp(string file, string appId)
        => Driver.InstallApp(file, appId);

    public Task KeyPress(char keyCode)
        => Driver.KeyPress(keyCode);

    public Task LaunchApp(string appId)
        => Driver.LaunchApp(appId);

    public Task LongPress(int x, int y)
        => Driver.LongPress(x, y);

    public Task OpenUri(string uri)
        => Driver.OpenUri(uri);

    public Task PullFile(string appId, string remoteFile, string localDirectory)
        => Driver.PullFile(appId, remoteFile, localDirectory);

    public Task PushFile(string appId, string localFile, string destinationDirectory)
        => Driver.PushFile(appId, localFile, destinationDirectory);

    public Task RemoveApp(string appId)
        => Driver.RemoveApp(appId);

    public Task StopApp(string appId)
        => Driver.StopApp(appId);

    public Task Swipe((int x, int y) start, (int x, int y) end)
        => Driver.Swipe(start, end);

    public Task Tap(int x, int y)
        => Driver.Tap(x, y);
}
