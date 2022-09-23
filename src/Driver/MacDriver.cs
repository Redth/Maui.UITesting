using Microsoft.Maui.Automation.Remote;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Microsoft.Maui.Automation.Driver;


public class MacDriver : Driver
{
	public MacDriver(IAutomationConfiguration configuration, ILoggerFactory? loggerProvider)
		: base(configuration, loggerProvider)
	{
		var port = configuration.AppAgentPort;
		var address = configuration.AppAgentAddress;


		Name = $"Mac ({configuration.Device})";

		if (string.IsNullOrEmpty(configuration.AppId))
			Configuration.AppId = AppUtil.GetBundleIdentifier(Configuration.AppFilename)
				?? throw new Exception("AppId not found");

		grpc = new GrpcHost(configuration, LoggerFactory);
	}

	readonly GrpcHost grpc;

	public override string Name { get; }

	public override Task ClearAppState()
		=> Task.CompletedTask;

	public override Task InstallApp()
		=> Task.CompletedTask;

	public override Task RemoveApp()
		=> Task.CompletedTask;

	public override Task<IDeviceInfo> GetDeviceInfo()
		=> Task.FromResult<IDeviceInfo>(new DeviceInfo());

	public override Task LaunchApp()
		=> Process.Start(new ProcessStartInfo("/usr/bin/open", $"-n \"{Configuration.AppFilename}\"")
		{
			CreateNoWindow = true,
			UseShellExecute = true
		})!.WaitForExitAsync();

	public override Task StopApp()
		=> Task.CompletedTask;

	public override Task OpenUri(string uri)
		=> Process.Start(new ProcessStartInfo("/usr/bin/open", $"{uri}")
		{
			CreateNoWindow = true,
			UseShellExecute = true
		})!.WaitForExitAsync();


	public override Task PushFile(string localFile, string destinationDirectory)
	{
		if (string.IsNullOrEmpty(Configuration.AppFilename))
			throw new FileNotFoundException("AppFilename");

		var bundleRoot = Path.Combine(Configuration.AppFilename, "Contents");
		var dest = Path.Combine(bundleRoot, Path.GetFileName(localFile));
		File.Copy(localFile, dest);
		return Task.CompletedTask;
	}

	public override Task PullFile(string remoteFile, string localDirectory)
	{
		if (string.IsNullOrEmpty(Configuration.AppFilename))
			throw new FileNotFoundException("AppFilename");

		var bundleRoot = Path.Combine(Configuration.AppFilename, "Contents");
		var src = Path.Combine(bundleRoot, remoteFile);
		var dest = Path.Combine(localDirectory, Path.GetFileName(remoteFile));
		File.Copy(src, dest);
		return Task.CompletedTask;
	}

	public override Task InputText(Element element, string text)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, Actions.InputText, element.Id, text);

	public override Task Back()
		=> Task.CompletedTask;

	public override Task KeyPress(char keyCode)
		=> Task.CompletedTask;

	public override Task Tap(int x, int y)
		=> Task.CompletedTask;

	public override Task Tap(Element element)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, Actions.Tap, element.Id);

	public override Task LongPress(int x, int y)
		=> Task.CompletedTask;

	public override Task LongPress(Element element)
			=> Tap(element);

	public override Task Swipe((int x, int y) start, (int x, int y) end)
		=> Task.CompletedTask;

	public override Task<string?> GetProperty(string elementId, string propertyName)
			=> grpc.Client.GetProperty(Configuration.AutomationPlatform, elementId, propertyName);

	public override Task<IEnumerable<Element>> GetElements()
		=> base.SetDriver(grpc.Client.GetElements(Configuration.AutomationPlatform));

	public override Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, action, elementId, arguments);

	public override Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args)
		=> grpc.Client.Backdoor(Configuration.AutomationPlatform, fullyQualifiedTypeName, staticMethodName, args);

	public override async void Dispose()
	{
		if (grpc is not null)
			await grpc.Stop();
	}
}
